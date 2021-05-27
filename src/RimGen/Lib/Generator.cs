using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace RimGen.Lib
{
    public class Generator
    {
        //0 - 1px
        //5 - 24px
        //10 - 48px
        //15 - 72px

        const int height = 24;
        const int width = 24;
        const int topAttrStepHeight = 27;
        const int leftOffset = 1029;

        const int firstAttrTopOffset = 336;

        const int shootingTopOffset = 336;
        const int craftTopOffset = 675;

        const int fire1LeftOffset = -13;//смещение 1 огонька влево
        const int fire1TopOffset = 10;

        const int fire2LeftOffset = -8;//смещение 2 огоньков влево
        const int fire2TopOffset = 13;

        static Color setColor = Color.FromArgb(-12566206); //"{Name=ff404142, ARGB=(255, 64, 65, 66)}"
        static Color notSetColor = Color.FromArgb(-14013652); //"{Name=ff2a2b2c, ARGB=(255, 42, 43, 44)}"

        static Color setFireColor = Color.FromArgb(-3229); //"{Name=fffff363, ARGB=(255, 255, 243, 99)}"

        public static string Generate(List<Condition> conditions, int maxSecondsTimeout = 30)
        {
            var resultMsg = "";

            try
            {
                Game.Init();
            }
            catch (Exception e)
            {
                return e.Message;
            }

            var start = true;
            var startDate = DateTime.Now;

            while (DateTime.Now < startDate.AddSeconds(maxSecondsTimeout))
            {
                if (start)
                {
                    Thread.Sleep(150);
                    start = false;
                }
                else
                {
                    Thread.Sleep(50);
                }

                Bitmap screen = null;
                
                try
                {
                    screen = Game.GetScreenshot();
                    var conditionsSucceed = 0;
                    foreach (var condition in conditions)
                    {
                        int valueOffset = condition.MinValue == 0 ? 1 : (int)Math.Ceiling(condition.MinValue * 3.8);
                        int attrTopOffset = firstAttrTopOffset + ((int)condition.Attr * 27);

                        //цвет навыка
                        var color = screen.GetPixel(leftOffset + valueOffset, attrTopOffset);

                        //цвет огонька
                        var fire1Color = screen.GetPixel(leftOffset + fire1LeftOffset, attrTopOffset + fire1TopOffset);
                        var fire2Color = screen.GetPixel(leftOffset + fire2LeftOffset, attrTopOffset + fire2TopOffset);

                        if (Form1.FireAttr)
                        {
                            if (color == setColor && (fire1Color == setFireColor || fire2Color == setFireColor))
                            {
                                conditionsSucceed++;
                            }
                        }
                        else
                        {
                            if (color == setColor)
                            {
                                conditionsSucceed++;
                            }
                        }
                    }

                    if (conditionsSucceed == conditions.Count)
                    {
                        if (Form1.SaveScreen)
                        {
                            SaveScreenForConditions(screen, conditions);
                        }
                        
                        resultMsg = Form1.Lang == "ru" ? "Готово" : "Done";
                        break;
                    }
                    else
                    {
                        Game.SendGenerateNew();
                    }
                }
                catch
                {
                    resultMsg = Form1.Lang == "ru" ? "Прервано" : "Interrupted";
                    break;
                }
                finally
                {
                    screen?.Dispose();
                }

                //var craftColor = screen.GetPixel(leftOffset + 72, craftTopOffset);
                //var argb = craftColor.ToArgb();

                //if (craftColor == setColor)
                //{
                //    isFound = true;
                //    Game.SaveScreenshot();


                //    Rectangle cropRect = new Rectangle(leftOffset - 140, craftTopOffset, 170, 24);
                //    Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);
                //    using (Graphics g = Graphics.FromImage(target))
                //    {
                //        g.DrawImage(screen, new Rectangle(0, 0, target.Width, target.Height),
                //                         cropRect,
                //                         GraphicsUnit.Pixel);
                //    }
                //    target.Save("crop.png", ImageFormat.Png);
                //    target.Dispose();

                //    break;
                //}
                //else
                //{
                //}
            }

            return resultMsg;
        }

        private static void SaveScreenForConditions(Bitmap screen, List<Condition> conditions)
        {
            string attrsString = "";
            foreach (var cond in conditions)
            {
                attrsString += $"{cond.Attr.ToString()}-{cond.MinValue}_";
            }
            string screenshotPath = $"rim_gen_{attrsString}_{DateTime.Now.ToShortDateString()}_{DateTime.Now:HH.mm.ss}.png";
            screen.Save(screenshotPath, ImageFormat.Png);
        }
    }
}
