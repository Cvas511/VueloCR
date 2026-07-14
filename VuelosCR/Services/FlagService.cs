using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;

namespace VuelosCRSA.Services
{
    public class FlagService
    {
        private static readonly Dictionary<string, string> IsoCodes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Argentina", "ar" }, { "Bolivia", "bo" }, { "Brasil", "br" }, { "Chile", "cl" },
            { "Colombia", "co" }, { "Ecuador", "ec" }, { "Guyana", "gy" }, { "Nicaragua", "ni" },
            { "México", "mx" }, { "Estados Unidos", "us" }, { "Japón", "jp" }, { "China", "cn" },
            { "Reino Unido", "gb" }, { "España", "es" }, { "Francia", "fr" }, { "Alemania", "de" },
            { "Italia", "it" }, { "Países Bajos", "nl" }, { "Suiza", "ch" }
        };

        private static readonly Dictionary<string, string> DisplayNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Argentina", "🇦🇷 Argentina" }, { "Bolivia", "🇧🇴 Bolivia" }, { "Brasil", "🇧🇷 Brasil" }, { "Chile", "🇨🇱 Chile" },
            { "Colombia", "🇨🇴 Colombia" }, { "Ecuador", "🇪🇨 Ecuador" }, { "Guyana", "🇬🇾 Guyana" }, { "Nicaragua", "🇳🇮 Nicaragua" },
            { "México", "🇲🇽 México" }, { "Estados Unidos", "🇺🇸 Estados Unidos" }, { "Japón", "🇯🇵 Japón" }, { "China", "🇨🇳 China" },
            { "Reino Unido", "🇬🇧 Reino Unido" }, { "España", "🇪🇸 España" }, { "Francia", "🇫🇷 Francia" }, { "Alemania", "🇩🇪 Alemania" },
            { "Italia", "🇮🇹 Italia" }, { "Países Bajos", "🇳🇱 Países Bajos" }, { "Suiza", "🇨🇭 Suiza" }
        };

        private static readonly Dictionary<string, string> ResourceNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Argentina", "flag_argentina" }, { "Bolivia", "flag_bolivia" }, { "Brasil", "flag_brasil" }, { "Chile", "flag_chile" },
            { "Colombia", "flag_colombia" }, { "Ecuador", "flag_ecuador" }, { "Guyana", "flag_guyana" }, { "Nicaragua", "flag_nicaragua" },
            { "México", "flag_mexico" }, { "Estados Unidos", "flag_usa" }, { "Japón", "flag_japan" }, { "China", "flag_china" },
            { "Reino Unido", "flag_uk" }, { "España", "flag_spain" }, { "Francia", "flag_france" }, { "Alemania", "flag_germany" },
            { "Italia", "flag_italy" }, { "Países Bajos", "flag_netherlands" }, { "Suiza", "flag_switzerland" }
        };

        public Image GetFlagImage(string country, int width, int height)
        {
            if (string.IsNullOrWhiteSpace(country)) return null;
            return LoadResourceFlag(country) ?? LoadCachedOrDownloadedFlag(country) ?? CreateFallbackFlag(country, width, height);
        }

        public string GetDisplayName(string country)
        {
            if (string.IsNullOrWhiteSpace(country)) return string.Empty;
            return DisplayNames.ContainsKey(country) ? DisplayNames[country] : country;
        }

        private Image LoadResourceFlag(string country)
        {
            var resName = ResourceNames.ContainsKey(country) ? ResourceNames[country] : null;
            if (string.IsNullOrEmpty(resName)) return null;
            return (Image)Properties.Resources.ResourceManager.GetObject(resName);
        }

        private Image LoadCachedOrDownloadedFlag(string country)
        {
            var code = GetIsoCode(country);
            if (string.IsNullOrEmpty(code)) return null;

            var flagsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "flags");
            if (!Directory.Exists(flagsDir)) Directory.CreateDirectory(flagsDir);

            var filePath = Path.Combine(flagsDir, code + ".png");
            if (File.Exists(filePath))
            {
                try
                {
                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        return Image.FromStream(fs);
                    }
                }
                catch
                {
                    try { File.Delete(filePath); } catch { }
                }
            }

            try
            {
                var url = $"https://flagcdn.com/w320/{code}.png";
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "Mozilla/5.0");
                    wc.DownloadFile(url, filePath);
                }
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    return Image.FromStream(fs);
                }
            }
            catch
            {
                try { if (File.Exists(filePath)) File.Delete(filePath); } catch { }
                return null;
            }
        }

        private string GetIsoCode(string country)
        {
            return IsoCodes.ContainsKey(country) ? IsoCodes[country] : null;
        }

        private Image CreateFallbackFlag(string country, int width, int height)
        {
            var bmp = new Bitmap(Math.Max(1, width), Math.Max(1, height));
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.White);

                switch (country)
                {
                    case "Argentina": DrawHorizontalBands(g, bmp, Color.FromArgb(116, 172, 223), Color.White, Color.FromArgb(116, 172, 223)); DrawCenteredCircle(g, bmp, Color.Gold); break;
                    case "Estados Unidos": DrawUsaFlag(g, bmp); break;
                    case "España": DrawHorizontalBands(g, bmp, Color.Red, Color.FromArgb(255, 204, 0), Color.Red); break;
                    case "Japón": DrawCenteredCircle(g, bmp, Color.Red, 0.5f); break;
                    case "China": g.Clear(Color.FromArgb(222, 41, 16)); DrawChinaStars(g, bmp); break;
                    case "Reino Unido": DrawUnionJack(g, bmp); break;
                    case "Brasil": g.Clear(Color.FromArgb(0, 156, 59)); DrawDiamond(g, bmp, Color.Gold); DrawCenteredCircle(g, bmp, Color.FromArgb(0, 39, 118), 0.45f); break;
                    case "Bolivia": DrawHorizontalBands(g, bmp, Color.FromArgb(204, 0, 0), Color.FromArgb(255, 204, 0), Color.FromArgb(0, 153, 51)); break;
                    case "Chile": DrawChileFlag(g, bmp); break;
                    case "Colombia": DrawColombiaFlag(g, bmp); break;
                    case "Ecuador": DrawColombiaFlag(g, bmp); break;
                    case "Guyana": DrawGuyanaFlag(g, bmp); break;
                    case "Nicaragua": DrawHorizontalBands(g, bmp, Color.FromArgb(0, 102, 204), Color.White, Color.FromArgb(0, 102, 204)); DrawCenteredCircle(g, bmp, Color.LightBlue, 0.35f); break;
                    case "México": DrawVerticalBands(g, bmp, Color.FromArgb(0, 104, 71), Color.White, Color.FromArgb(206, 17, 38)); DrawCenteredCircle(g, bmp, Color.DarkGreen, 0.25f); break;
                    case "Francia": DrawVerticalBands(g, bmp, Color.FromArgb(0, 85, 164), Color.White, Color.FromArgb(239, 65, 53)); break;
                    case "Alemania": DrawHorizontalBands(g, bmp, Color.Black, Color.FromArgb(221, 0, 0), Color.FromArgb(255, 204, 0)); break;
                    case "Italia": DrawVerticalBands(g, bmp, Color.FromArgb(0, 146, 70), Color.White, Color.FromArgb(206, 43, 55)); break;
                    case "Países Bajos": DrawHorizontalBands(g, bmp, Color.FromArgb(174, 28, 40), Color.White, Color.FromArgb(33, 70, 139)); break;
                    case "Suiza": g.Clear(Color.FromArgb(206, 17, 38)); DrawSwissCross(g, bmp); break;
                    default: DrawPlaceholder(g, bmp, country); break;
                }
            }

            return bmp;
        }

        private void DrawPlaceholder(Graphics g, Bitmap bmp, string country)
        {
            using (var brush = new SolidBrush(Color.LightGray)) g.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
            using (var pen = new Pen(Color.DarkGray, 2)) g.DrawRectangle(pen, 1, 1, bmp.Width - 3, bmp.Height - 3);
            using (var font = new Font("Segoe UI", Math.Max(8, bmp.Width / 15)))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var brush = new SolidBrush(Color.Black))
            {
                g.DrawString(country, font, brush, new RectangleF(0, 0, bmp.Width, bmp.Height), sf);
            }
        }

        private void DrawHorizontalBands(Graphics g, Bitmap bmp, Color top, Color middle, Color bottom)
        {
            var h = bmp.Height;
            var w = bmp.Width;
            using (var b = new SolidBrush(top)) g.FillRectangle(b, 0, 0, w, h / 3);
            using (var b = new SolidBrush(middle)) g.FillRectangle(b, 0, h / 3, w, h / 3);
            using (var b = new SolidBrush(bottom)) g.FillRectangle(b, 0, 2 * h / 3, w, h / 3);
        }

        private void DrawVerticalBands(Graphics g, Bitmap bmp, Color left, Color middle, Color right)
        {
            var w = bmp.Width;
            var h = bmp.Height;
            using (var b = new SolidBrush(left)) g.FillRectangle(b, 0, 0, w / 3, h);
            using (var b = new SolidBrush(middle)) g.FillRectangle(b, w / 3, 0, w / 3, h);
            using (var b = new SolidBrush(right)) g.FillRectangle(b, 2 * w / 3, 0, w / 3, h);
        }

        private void DrawCenteredCircle(Graphics g, Bitmap bmp, Color color, float relativeSize = 0.3f)
        {
            var diameter = Math.Min(bmp.Width, bmp.Height) * relativeSize;
            var rect = new RectangleF((bmp.Width - diameter) / 2f, (bmp.Height - diameter) / 2f, diameter, diameter);
            using (var b = new SolidBrush(color)) g.FillEllipse(b, rect);
        }

        private void DrawDiamond(Graphics g, Bitmap bmp, Color color)
        {
            var points = new[]
            {
                new PointF(bmp.Width * 0.5f, bmp.Height * 0.1f),
                new PointF(bmp.Width * 0.9f, bmp.Height * 0.5f),
                new PointF(bmp.Width * 0.5f, bmp.Height * 0.9f),
                new PointF(bmp.Width * 0.1f, bmp.Height * 0.5f)
            };
            using (var b = new SolidBrush(color)) g.FillPolygon(b, points);
        }

        private void DrawChileFlag(Graphics g, Bitmap bmp)
        {
            var sqW = (int)(bmp.Width * 0.33f);
            using (var b = new SolidBrush(Color.FromArgb(0, 56, 168))) g.FillRectangle(b, 0, 0, sqW, bmp.Height);
            using (var b = new SolidBrush(Color.White)) g.FillRectangle(b, sqW, 0, bmp.Width - sqW, bmp.Height / 2);
            using (var b = new SolidBrush(Color.FromArgb(206, 17, 38))) g.FillRectangle(b, sqW, bmp.Height / 2, bmp.Width - sqW, bmp.Height / 2);
            using (var star = new SolidBrush(Color.White)) g.FillEllipse(star, sqW * 0.25f - 5, bmp.Height * 0.4f - 10, 20, 20);
        }

        private void DrawColombiaFlag(Graphics g, Bitmap bmp)
        {
            using (var b = new SolidBrush(Color.FromArgb(255, 209, 0))) g.FillRectangle(b, 0, 0, bmp.Width, bmp.Height * 50 / 100);
            using (var b = new SolidBrush(Color.FromArgb(0, 56, 168))) g.FillRectangle(b, 0, bmp.Height * 50 / 100, bmp.Width, bmp.Height * 25 / 100);
            using (var b = new SolidBrush(Color.FromArgb(206, 17, 38))) g.FillRectangle(b, 0, bmp.Height * 75 / 100, bmp.Width, bmp.Height * 25 / 100);
        }

        private void DrawGuyanaFlag(Graphics g, Bitmap bmp)
        {
            g.Clear(Color.FromArgb(0, 153, 51));
            var points = new[] { new PointF(0, bmp.Height * 0.1f), new PointF(bmp.Width * 0.6f, bmp.Height * 0.5f), new PointF(0, bmp.Height * 0.9f) };
            using (var b = new SolidBrush(Color.FromArgb(255, 204, 0))) g.FillPolygon(b, points);
            using (var b = new SolidBrush(Color.FromArgb(206, 17, 38)))
            {
                var inner = new[] { new PointF(0, bmp.Height * 0.2f), new PointF(bmp.Width * 0.5f, bmp.Height * 0.5f), new PointF(0, bmp.Height * 0.8f) };
                g.FillPolygon(b, inner);
            }
        }

        private void DrawSwissCross(Graphics g, Bitmap bmp)
        {
            var bar = Math.Min(bmp.Width, bmp.Height) / 5f;
            using (var b = new SolidBrush(Color.White))
            {
                g.FillRectangle(b, bmp.Width * 0.4f - bar / 2, bmp.Height * 0.15f, bar, bmp.Height * 0.7f);
                g.FillRectangle(b, bmp.Width * 0.15f, bmp.Height * 0.4f - bar / 2, bmp.Width * 0.7f, bar);
            }
        }

        private void DrawUsaFlag(Graphics g, Bitmap bmp)
        {
            for (int i = 0; i < 13; i++)
            {
                var color = i % 2 == 0 ? Color.FromArgb(178, 34, 52) : Color.White;
                using (var b = new SolidBrush(color)) g.FillRectangle(b, 0, i * (bmp.Height / 13f), bmp.Width, bmp.Height / 13f + 1);
            }

            var cantonH = (int)(bmp.Height * 7f / 13f);
            var cantonW = (int)(bmp.Width * 0.4f);
            using (var b = new SolidBrush(Color.FromArgb(10, 49, 97))) g.FillRectangle(b, 0, 0, cantonW, cantonH);
            using (var starBrush = new SolidBrush(Color.White))
            {
                var rows = 5;
                var cols = 6;
                var padX = cantonW / (cols + 1f);
                var padY = cantonH / (rows + 1f);
                for (int r = 1; r <= rows; r++)
                for (int c = 1; c <= cols; c++)
                {
                    var cx = c * padX - padX / 2;
                    var cy = r * padY - padY / 2;
                    g.FillEllipse(starBrush, cx - 3, cy - 3, 6, 6);
                }
            }
        }

        private void DrawChinaStars(Graphics g, Bitmap bmp)
        {
            using (var b = new SolidBrush(Color.Gold))
            {
                g.FillEllipse(b, bmp.Width * 0.08f, bmp.Height * 0.08f, bmp.Width * 0.18f, bmp.Height * 0.18f);
                var s = Math.Min(bmp.Width, bmp.Height) * 0.06f;
                var sx = bmp.Width * 0.6f;
                var sy = bmp.Height * 0.12f;
                g.FillEllipse(b, sx, sy, s, s);
                g.FillEllipse(b, sx + s * 1.6f, sy + s * 0.8f, s, s);
                g.FillEllipse(b, sx + s * 1.6f, sy + s * 2.4f, s, s);
                g.FillEllipse(b, sx, sy + s * 3.2f, s, s);
            }
        }

        private void DrawUnionJack(Graphics g, Bitmap bmp)
        {
            g.Clear(Color.FromArgb(1, 33, 105));
            using (var white = new SolidBrush(Color.White))
            using (var red = new SolidBrush(Color.Red))
            {
                var penW = new Pen(white, Math.Max(6, Math.Min(bmp.Width, bmp.Height) / 10));
                var penR = new Pen(red, Math.Max(3, Math.Min(bmp.Width, bmp.Height) / 18));
                g.DrawLine(penW, 0, 0, bmp.Width, bmp.Height);
                g.DrawLine(penW, bmp.Width, 0, 0, bmp.Height);
                g.DrawLine(penR, 0, 0, bmp.Width * 0.6f, bmp.Height * 0.4f);
                g.DrawLine(penR, bmp.Width, 0, bmp.Width * 0.4f, bmp.Height * 0.4f);
                g.DrawLine(penR, 0, bmp.Height, bmp.Width * 0.4f, bmp.Height * 0.6f);
                g.DrawLine(penR, bmp.Width, bmp.Height, bmp.Width * 0.6f, bmp.Height * 0.6f);
                g.FillRectangle(white, bmp.Width * 0.45f, 0, bmp.Width * 0.1f, bmp.Height);
                g.FillRectangle(white, 0, bmp.Height * 0.45f, bmp.Width, bmp.Height * 0.1f);
                g.FillRectangle(red, bmp.Width * 0.475f, 0, bmp.Width * 0.05f, bmp.Height);
                g.FillRectangle(red, 0, bmp.Height * 0.475f, bmp.Width, bmp.Height * 0.05f);
            }
        }
    }
}
