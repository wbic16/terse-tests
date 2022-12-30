using TerseNotepad;

namespace TerseNotepadTest
{
    public class CoordinatesTest
    {
        [Fact]
        public void CoordinateFormat()
        {
            var test1 = "1-2-3-4-5-6-7-8-9";
            var expected1 = "p1g2s3y4h5e6w7i8m9";
            var result1 = new Coordinates(test1).ToString();
            Assert.Equal(expected1, result1);

            var test2 = "p9-g8-s7-y6-h5-e4-w3-i2-m1";
            var expected2 = "p9g8s7y6h5e4w3i2m1";
            var result2 = new Coordinates(test2).ToString();
            Assert.Equal(expected2, result2);
        }
    }
}
