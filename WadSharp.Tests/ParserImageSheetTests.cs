using WadSharp.Parsing;

namespace WadSharp.Tests
{
    public class ParserImageSheetTests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Try to create a sheet with one image and see if it is added correctly.
        /// </summary>
        [Test]
        public void AssertSheetIsCreatedWithImage()
        {
            SheetCollection sheets = SheetCollection.Create(new List<ParserImage>()
            {
                new ParserImage()
                {
                    Name = "TestImage",
                    Width = 128,
                    Height = 128,
                    Data = new byte[128 * 128 * 4]
                }
            });

            Assert.True(sheets.Sheets.Count == 1);
            Sheet sheet = sheets.Sheets[0];
            Assert.True(sheet.Sprites.Count == 1);

            SheetSprite sprite = sheet.Sprites[0];
            Assert.That(sprite.Image.Name , Is.EqualTo("TestImage"));
            Assert.That(sprite.X, Is.EqualTo(1));
            Assert.That(sprite.Y , Is.EqualTo(1));
            Assert.That(sprite.Width, Is.EqualTo(128));
            Assert.That(sprite.Height, Is.EqualTo(128));
            Assert.That(sprite.X / (float)sheet.Width, Is.EqualTo(sprite.U0));
            Assert.That(sprite.Y / (float)sheet.Height, Is.EqualTo(sprite.V0));
            Assert.That((sprite.X + sprite.Width) / (float)sheet.Width, Is.EqualTo(sprite.U1));
            Assert.That((sprite.Y + sprite.Height) / (float)sheet.Height, Is.EqualTo(sprite.V1));
        }

        /// <summary>
        /// Try to create a sheet with three images and see if it is added correctly.
        /// </summary>
        [Test]
        public void AssertSheetIsCreatedWithThreeImages()
        {
            SheetCollection sheets = SheetCollection.Create(new List<ParserImage>()
            {
                new ParserImage()
                {
                    Name = "TestImage1",
                    Width = 500,
                    Height = 500,
                    Data = new byte[500 * 500 * 4]
                },
                new ParserImage()
                {
                    Name = "TestImage2",
                    Width = 500,
                    Height = 500,
                    Data = new byte[500 * 500 * 4]
                },
                new ParserImage()
                {
                    Name = "TestImage3",
                    Width = 500,
                    Height = 500,
                    Data = new byte[500 * 500 * 4]
                },

            }, 1024, 1024);

            Assert.True(sheets.Sheets.Count == 1, $"Expected sheet count to be 1. Found count {sheets.Sheets.Count}.");
            Sheet sheet = sheets.Sheets[0];
            Assert.True(sheet.Sprites.Count == 3);

            SheetSprite sprite = sheet.Sprites[0];
            Assert.True(sprite.Image.Name == "TestImage1");
            Assert.True(sprite.X == 1);
            Assert.True(sprite.Y == 1);
            Assert.True(sprite.Width == 500);
            Assert.True(sprite.Height == 500);

            sprite = sheet.Sprites[1];
            Assert.True(sprite.Image.Name == "TestImage2");
            Assert.True(sprite.X == 503, $"Expected TestImage2 X to be at 502. Current X {sprite.X}.");
            Assert.True(sprite.Y == 1);
            Assert.True(sprite.Width == 500);
            Assert.True(sprite.Height == 500);

            sprite = sheet.Sprites[2];
            Assert.True(sprite.Image.Name == "TestImage3");
            Assert.True(sprite.X == 1);
            Assert.True(sprite.Y == 502, $"Expected TestImage3 Y to be at 501. Current Y {sprite.Y}.");
            Assert.True(sprite.Width == 500);
            Assert.True(sprite.Height == 500);

        }
    }
}