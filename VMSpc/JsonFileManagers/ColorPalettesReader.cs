using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VMSpc.JsonFileManagers
{
    public class ColorPalette
    {
        public int
            Id;
        public string
            PaletteName;
        public Color
            MainBackground,
            GaugeBackground,
            Captions,
            GaugeText,
            Green,
            Yellow,
            Red;
        public void Copy(ColorPalette colorPalette)
        {
            Id = colorPalette.Id;
            PaletteName = colorPalette.PaletteName;
            MainBackground = colorPalette.MainBackground;
            GaugeBackground = colorPalette.GaugeBackground;
            Captions = colorPalette.Captions;
            GaugeText = colorPalette.GaugeText;
            Green = colorPalette.Green;
            Yellow = colorPalette.Yellow;
            Red = colorPalette.Red;
        }

        /// <summary>
        /// Loads default values for a new color palette
        /// </summary>
        public void LoadDefaults()
        {
            PaletteName = "New Color Palette";
            MainBackground = Colors.White;
            GaugeBackground = Colors.LightSkyBlue;
            Captions = Colors.Black;
            GaugeText = Colors.Black;
            Green = Colors.Green;
            Yellow = Colors.Yellow;
            Red = Colors.Red;
        }
    }
    public class ColorPalettesContents : IJsonContents
    {
        public List<ColorPalette> ColorPaletteList;
    }

    public class ColorPalettesReader : JsonFileReader<ColorPalettesContents>
    {
        private ScreenContents Screen => ConfigurationManager.ConfigManager.Screen.Contents;
        public ColorPalettesReader() : base("\\configuration\\colorPalettes.json")
        {
        }

        protected override ColorPalettesContents GetDefaultContents()
        {

            return new ColorPalettesContents()
            {
                ColorPaletteList = new List<ColorPalette>()
                {
                    new ColorPalette()
                    {
                        Id = 1,
                        PaletteName = "Default",
                        MainBackground = Colors.Teal,
                        GaugeBackground = Colors.White,
                        Captions = Colors.Blue,
                        GaugeText = Colors.Black,
                        Green = Colors.Green,
                        Yellow = Colors.Yellow,
                        Red = Colors.Red,
                    },
                },
            };
        }

        public ColorPalette GetSelectedPalette()
        {
            foreach (var palette in Contents.ColorPaletteList)
            {
                if (palette.Id == Screen.selectedColorPaletteId)
                    return palette;
            }
            return Contents.ColorPaletteList[0];
        }

        public void AddNewPalette(ColorPalette palette)
        {
            palette.Id = Contents.ColorPaletteList.Count() + 1;
            Contents.ColorPaletteList.Add(palette);
        }

        public void DeletePalette(int Id)
        {
            //remove palette from the list
            foreach (var palette in Contents.ColorPaletteList)
            {
                if (palette.Id == Id)
                {
                    Contents.ColorPaletteList.Remove(palette);
                    break;
                }
            }
            //reset all remaining palette Ids to keep them sequential
            for (int i = 1; i <= Contents.ColorPaletteList.Count; i++)
            {
                Contents.ColorPaletteList[i - 1].Id = i;
            }
        }
    }
}
