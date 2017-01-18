using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        const int leftOffset = 980;

        const int firstAttrTopOffset = 405;

        const int shootingTopOffset = 405;
        const int craftTopOffset = 675;

        static Color setColor = Color.FromArgb(-12566206); //"{Name=ff404142, ARGB=(255, 64, 65, 66)}"
        static Color notSetColor = Color.FromArgb(-14013652); //"{Name=ff2a2b2c, ARGB=(255, 42, 43, 44)}"

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
                Game.SendGenerateNew();
                if (start)
                {
                    Thread.Sleep(100);
                    start = false;
                }
                else
                {
                    Thread.Sleep(15);
                }

                Bitmap screen = null;
                try
                {
                    screen = Game.GetScreenshot();
                    var conditionsSucceed = 0;
                    foreach (var condition in conditions)
                    {
                        int valueOffset = condition.MinValue == 0 ? 1 : (int)Math.Ceiling(condition.MinValue * 4.8);
                        int attrTopOffset = firstAttrTopOffset + ((int)condition.Attr * 27);

                        var color = screen.GetPixel(leftOffset + valueOffset, attrTopOffset);
                        if (color == setColor)
                        {
                            conditionsSucceed++;
                        }
                    }

                    if (conditionsSucceed == conditions.Count)
                    {
                        SaveScreenForConditions(screen, conditions);
                        resultMsg = Form1.Lang == "ru" ? "Готово" : "Done";
                        break;
                    }
                }
                catch (Exception e)
                {
                    resultMsg = Form1.Lang == "ru" ? "Прервано" : "Interrupted";
                    break;
                }
                finally
                {
                    if (screen != null)
                    {
                        screen.Dispose();
                    }
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

                screen.Dispose();
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
            string screenshotPath = $"rim_gen_{attrsString}_{DateTime.Now.ToShortDateString()}_{DateTime.Now.ToString("HH.mm.ss")}.png";
            screen.Save(screenshotPath, ImageFormat.Png);
        }
    }
}
