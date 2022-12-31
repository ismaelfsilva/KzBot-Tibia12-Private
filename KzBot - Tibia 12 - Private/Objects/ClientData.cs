using KzBot.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzBot.Objects
{
    public static class ClientData
    {


        public static Rectangle GameMapRect { get; set; } = new Rectangle();
        public static Size SqmSize { get; set; } = new Size();
        public static Point GameMapCenter { get; set; } = new Point();



        private static Point TradeWindowPoint { get; set; } = new Point();

        // COOLDOWN
        public static bool hasAttackCooldown { get; set; } = false;
        public static bool hasHealCooldown { get; set; } = false;
        public static bool hasSupportCooldown { get; set; } = false;
        public static bool hasStrongHealCooldown { get; set; } = false;
        public static bool hasMagicShieldCooldown { get; set; } = true;


        // EQUIPMENT
        public static bool hasAmulet = false;
        public static bool hasRing = false;

        // STATUS
        public static bool isManaShielded = false;
        public static bool isParalyzed = false;
        public static bool isHasted = false;
        public static bool isBuffed = false;
        public static bool isHungry = false;
        public static bool isPzLocked = false;

        public static bool hasMessageLocalChat = false;








        // INTERNAL
        private static Color _colorAmulet = Color.White;
        private static Point _hasAmulet = new Point(-151, 185); // NO AMULET | R == 82

        private static Color _colorRing = Color.White;
        private static Point _hasRing = new Point(-151, 248); // NO RING | R == 88

        private static Point _attackCooldown = new Point(0, 0);
        private static Point _healCooldown = new Point(0, 0);
        private static Point _exuraMaxVitaCooldown = new Point(0, 0);
        private static Point _supportCooldown = new Point(0, 0);
        private static Point _utamoVitaCooldown = new Point(0, 0);

        private static Point _hasMessageLocalChat = new Point(0, 0); // atkCd + 30, 14 || R != 127

        private static Point _statusStart = new Point(0, 0);

        private static bool foundGroupCds = false;
        private static bool foundUtamoVitaCd = false;
        private static bool foundMaxVitaCd = false;

        public static void FindPositions()
        {
            Bitmap clientImage = new Bitmap(ScreenCapture.CaptureWindow(Globals.Process.MainWindowHandle));

            _hasAmulet = new Point(clientImage.Width - 151, 185);
            _colorAmulet = clientImage.GetPixel(_hasAmulet.X, _hasAmulet.Y);

            _hasRing = new Point(clientImage.Width - 151, 248);
            _colorRing = clientImage.GetPixel(_hasRing.X, _hasRing.Y);

            _statusStart = new Point(clientImage.Width - 162, 297);

            for (int x = 0; x < clientImage.Width; x++)
            {
                for (int y = 0; y < clientImage.Height; y++)
                {
                    Color pixelColor = clientImage.GetPixel(x, y);
                    if (!foundGroupCds && pixelColor.R == 89 && pixelColor.G == 89 && pixelColor.B == 89)
                    {
                        for (int cdSquares = 0; cdSquares < 7; cdSquares++)
                        {
                            if (x + (cdSquares * 25) >= clientImage.Width)
                                break;

                            Color stepPixelColor = clientImage.GetPixel(x + (cdSquares * 25), y);
                            if (stepPixelColor.R != 89 || stepPixelColor.G != 89 || stepPixelColor.B != 89)
                            {
                                break;
                            }
                            else if (cdSquares == 6)
                            {
                                _attackCooldown = new Point(x + 5, y - 1);
                                _healCooldown = new Point(x + 5 + 25, y - 1);
                                _supportCooldown = new Point(x + 5 + 50, y - 1);

                                _hasMessageLocalChat = new Point(x + 30, y + 15);

                                foundGroupCds = true;
                                break;
                            }
                        }
                    }
                    else if (!foundUtamoVitaCd && pixelColor.R == 193 && pixelColor.G == 211 && pixelColor.B == 207)
                    {
                        Color stepPixelColor = clientImage.GetPixel(x + 1, y);

                        if (stepPixelColor.R == 248 && stepPixelColor.G == 158 && stepPixelColor.B == 223)
                        {
                            _utamoVitaCooldown = new Point(x, y);
                            foundUtamoVitaCd = true;
                        }
                    }
                    else if (!foundMaxVitaCd && pixelColor.R == 58 && pixelColor.G == 88 && pixelColor.B == 245)
                    {
                        Color stepPixelColor = clientImage.GetPixel(x + 1, y);

                        if (stepPixelColor.R == 129 && stepPixelColor.G == 179 && stepPixelColor.B == 248)
                        {
                            _exuraMaxVitaCooldown = new Point(x, y);
                            foundMaxVitaCd = true;
                        }
                    }
                }
            }

            clientImage.Dispose();
        }

        public static void FindGameMapRect()
        {
            int left = 0, top = 4, right = 0, bottom = 0;

            using (Bitmap clientImage = (Bitmap)ScreenCapture.CaptureWindow(Globals.Process.MainWindowHandle))
            {
                if (clientImage == null)
                    return;

                WinApi.RECT windowRECT = Globals.clientRect;

                for (int x = 0; x < windowRECT.right; x++)
                {
                    Color p = clientImage.GetPixel(x, top);

                    if (Math.Abs(p.R - 22) < 10 && Math.Abs(p.G - 22) < 10 && Math.Abs(p.B - 22) < 10)
                    {
                        right = x;
                        if (left == 0)
                        {
                            Color firstP = clientImage.GetPixel(x, top + 10);
                            if (Math.Abs(firstP.R - 22) < 10 && Math.Abs(firstP.G - 22) < 10 && Math.Abs(firstP.B - 22) < 10)
                                left = x;
                        }
                    }
                    else if (left != 0)
                        break;
                }

                for (int y = top; y < windowRECT.bottom; y++)
                {
                    Color p = clientImage.GetPixel(left, y);

                    if (Math.Abs(p.R - 22) < 10 && Math.Abs(p.G - 22) < 10 && Math.Abs(p.B - 22) < 10)
                        bottom = y;
                    else
                        break;
                }
            }

            GameMapRect = new Rectangle(left, top, right-left, bottom-top);
            SqmSize = new Size((GameMapRect.Width - 2) / 15, (GameMapRect.Height - 2) / 11);
            GameMapCenter = new Point(GameMapRect.Left + GameMapRect.Width / 2, GameMapRect.Top + GameMapRect.Height / 2);
        }

        public static void Update()
        {
            using (Bitmap clientImage = (Bitmap)ScreenCapture.CaptureWindow(Globals.Process.MainWindowHandle))
            {
                if (clientImage == null)
                    return;                
                
                // Amulet
                Color amuletColor = clientImage.GetPixel(_hasAmulet.X, _hasAmulet.Y);
                hasAmulet = amuletColor.R != 82;

                // Ring
                Color ringColor = clientImage.GetPixel(_hasRing.X, _hasRing.Y);
                hasRing = ringColor.R != 88;

                // Has Message Local Chat
                Color messageLocalChat = clientImage.GetPixel(_hasMessageLocalChat.X, _hasMessageLocalChat.Y);
                hasMessageLocalChat = messageLocalChat.R != 127;

                bool _isHasted = false;
                bool _isParalyzed = false;
                bool _isManaShielded = false;
                bool _isHungry = false;
                bool _isPzLocked = false;
                bool _isBuffed = false;

                for (int x = 0; x < 10; x++)
                {
                    Color pixelColor = clientImage.GetPixel(_statusStart.X + (x * 10), _statusStart.Y);

                    if (pixelColor.R == 86) // 137, 159, 145 | 86, 97, 91 - Mana Shield (Mage)
                    {
                        isManaShielded = true;
                        _isManaShielded = true;
                    }
                    else if (pixelColor.B == 196) // 92, 106, 220 | 64, 74, 212 - Mana Shield (Energy Ring)
                    {
                        isManaShielded = true;
                        _isManaShielded = true;
                    }
                    else if (pixelColor.R == 129) // 129, 101, 58 - Haste
                    {
                        isHasted = true;
                        _isHasted = true;
                    }
                    else if (pixelColor.R == 255 && pixelColor.G == 0 && pixelColor.B == 0) // 255, 0, 0 | 233, 4, 4 - Paralyze
                    {
                        isParalyzed = true;
                        _isParalyzed = true;
                    }
                    else if (pixelColor.R == 246) // 246, 212, 143 - Hungry
                    {
                        isHungry = true;
                        _isHungry = true;
                    }
                    else if (pixelColor.R == 127) //  PzLock - 127, 0, 0
                    {
                        isPzLocked = true;
                        _isPzLocked = true;
                    }
                    else if (pixelColor.R == 37) //  Buffed -  38, 200, 16
                    {
                        isBuffed = true;
                        _isBuffed = true;
                    }
                }

                isManaShielded = _isManaShielded;
                isHasted = _isHasted && !_isParalyzed;
                isParalyzed = _isParalyzed;
                isHungry = _isHungry;
                isPzLocked = _isPzLocked;
                isBuffed = _isBuffed;
            }
        }


        public static Point FindOkButton()
        {
            using (Bitmap clientImage = (Bitmap)ScreenCapture.CaptureWindow(Globals.Process.MainWindowHandle))
            {
                if (clientImage == null)
                    return new Point();

                List<Point> npcTradePoints = new List<Point>() {
                new Point(18,7),
                new Point(18,12),
                new Point(16,10),
                new Point(26,12),
                new Point(23,7),
                new Point(23,12),
                };

                for (int x = clientImage.Width - 25; x > 0 ; x--)
                {
                    for (int y = clientImage.Height - 25; y > 0; y--)
                    {
                        Color p = clientImage.GetPixel(x, y);

                        if (p.R == 114 && p.G == 115 && p.B == 115)
                        {
                            bool wrongColor = false;
                            foreach (Point point in npcTradePoints)
                            {
                                Color pp = clientImage.GetPixel(x + point.X, y + point.Y);

                                if (pp.R != 192 || pp.G != 192)
                                {
                                    wrongColor = true;
                                    break;
                                }
                            }

                            if (!wrongColor)
                            {
                                TradeWindowPoint = new Point(x + 5, y + 3);
                                return TradeWindowPoint;
                            }
                        }
                    }
                }

            }
            return TradeWindowPoint;
        }

        public static Point FindTrade()
        {
            using (Bitmap clientImage = (Bitmap)ScreenCapture.CaptureWindow(Globals.Process.MainWindowHandle))
            {
                if (clientImage == null)
                    return new Point();

                List<Point> npcTradePoints = new List<Point>() {
                new Point(22,1),
                new Point(35,0),
                new Point(56,-1),
                };

                for (int x = clientImage.Width - 60; x > 0 ; x--)
                {
                    for (int y = clientImage.Height - 60; y > 0 ; y--)
                    {
                        Color p = clientImage.GetPixel(x, y);

                        if (p.R == 144 && p.G == 144 && p.B == 144)
                        {
                            bool wrongColor = false;
                            foreach (Point point in npcTradePoints)
                            {
                                Color pp = clientImage.GetPixel(x + point.X, y + point.Y);

                                if (pp.R != 144 || pp.G != 144 || pp.B != 144)
                                {
                                    wrongColor = true;
                                    break;
                                }
                            }

                            if (!wrongColor)
                            {
                                Color goldCoinColor = clientImage.GetPixel(x + 83, y + 33);
                                if (goldCoinColor.R == 181 && goldCoinColor.G == 98 && goldCoinColor.B == 16)
                                {
                                    TradeWindowPoint = new Point(x, y);
                                    return TradeWindowPoint;
                                }
                            }
                        }
                    }
                }

            }
            return TradeWindowPoint;
        }
    }
}
