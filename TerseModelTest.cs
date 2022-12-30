#pragma warning disable CA1416 // Validate platform compatibility
using TerseNotepad;

namespace TerseNotepadTest
{
    public class TerseModelTest
    {
        [Fact]
        public void Test_OneLine()
        {
            var model = new TerseNotepad.TerseModel();
            var expected = "Hello World\n";
            model.Load(expected, false);
            var test = model.Terse.getScroll();
            Assert.Equal(expected, test);
            Assert.Equal(1, model.LeafCount);
            Assert.Equal(2, model.WordCount);
        }

        [Fact]
        public void Test_OnePage()
        {
            var model = new TerseNotepad.TerseModel();
            var expected = @"Hello World
This is a multi-line dataset.

With extra line breaks.
";
            model.Load(expected, false);
            var test = model.Terse.getScroll();
            Assert.Equal(expected, test);
            Assert.Equal(1, model.LeafCount);
            Assert.Equal(11, model.WordCount);
        }

        [Fact]
        public void Test_TwoPages()
        {
            var model = new TerseNotepad.TerseModel();
            var page1 = "Line 1 Page 1\nLine 2 Page 1\n";
            var page2 = "Line 1 Page 2\nLine 2 Page 2\n";
            var data = $"{page1}\x17{page2}";
            model.Load(data, false);
            model.Coords.Scroll = 1;
            Assert.Equal(page1, model.Terse.getScroll());
            model.Coords.Scroll = 2;
            Assert.Equal(page2, model.Terse.getScroll());
            Assert.Equal(2, model.LeafCount);
            Assert.Equal(16, model.WordCount);
        }

        [Fact]
        public void Test_TwoSections()
        {
            var model = new TerseNotepad.TerseModel();
            var section1 = "Line 1 Page 1 Section 1\x17Line 1 Page 2 Section 1\n";
            var section2 = "Line 1 Page 1 Section 2\x17Line 1 Page 2 Section 2\n";
            var data = $"{section1}\x18{section2}";
            model.Load(data, false);
            model.Coords.Chapter = 1;
            model.Coords.Section = 1;
            model.Coords.Scroll = 1;
            var scroll1 = "Line 1 Page 1 Section 1";
            Assert.Equal(scroll1, model.Terse.getScroll());
            model.Coords.Section = 2;
            model.Coords.Scroll = 2;
            var scroll2 = "Line 1 Page 2 Section 2\n";
            Assert.Equal(scroll2, model.Terse.getScroll());
            Assert.Equal(4, model.LeafCount);
            Assert.Equal(24, model.WordCount);
        }

        [Fact]
        public void Test_TwoChapters()
        {
            var model = new TerseNotepad.TerseModel();
            var chapter1 = "Line 1 Page 1 Section 1 Chapter 1\x18Line 1 Page 1 Section 2 Chapter 1\n";
            var chapter2 = "Line 1 Page 1 Section 1 Chapter 2\x18Line 1 Page 1 Section 2 Chapter 2\n";
            var data = $"{chapter1}\x19{chapter2}";
            model.Load(data, false);
            model.Coords.Chapter = 1;
            model.Coords.Section = 1;
            model.Coords.Scroll = 1;
            var scroll1 = "Line 1 Page 1 Section 1 Chapter 1";
            Assert.Equal(scroll1, model.Terse.getScroll());
            model.Coords.Section = 1;
            model.Coords.Scroll = 1;
            model.Coords.Chapter = 2;
            var scroll2 = "Line 1 Page 1 Section 1 Chapter 2";
            Assert.Equal(scroll2, model.Terse.getScroll());
            Assert.Equal(4, model.LeafCount);
            Assert.Equal(32, model.WordCount);
        }

        [Fact]
        public void Test_FullTwoChapters()
        {
            var model = new TerseNotepad.TerseModel();
            var text = "Chapter 1, Section 1, Scroll 1\x17" +
                       "Chapter 1, Section 1, Scroll 2\x18" +
                       "Chapter 1, Section 2, Scroll 1\x17" +
                       "Chapter 1, Section 2, Scroll 2\x17\x18\x19" +
                       "Chapter 2, Section 1, Scroll 1\x17" +
                       "Chapter 2, Section 1, Scroll 2\x17\x18" +
                       "Chapter 2, Section 2, Scroll 1\x17" +
                       "Chapter 2, Section 2, Scroll 2\x17";
            model.Load(text, false);
            for (short scroll = 1; scroll < 3; ++scroll)
            {
                for (short section = 1; section < 3; ++section)
                {
                    for (short chapter = 1; chapter < 3; ++chapter)
                    {
                        model.Coords.Chapter = chapter;
                        model.Coords.Section = section;
                        model.Coords.Scroll = scroll;
                        var page = model.Terse.getScroll();
                        Assert.Equal($"Chapter {chapter}, Section {section}, Scroll {scroll}", page);
                    }
                }
            }

            Assert.Equal(8, model.LeafCount);
            Assert.Equal(48, model.WordCount);
        }

        [Fact]
        public void Test_Serialize()
        {
            var model = new TerseNotepad.TerseModel();
            model.Terse.Root = new();
            var library = model.Terse.Root.Library[1] = new();
            var shelf = library.Shelf[1] = new();
            var series = shelf.Series[1] = new();
            var collection = series.Collection[1] = new();
            var volume = collection.Volume[1] = new();
            var book = volume.Book[1] = new();
            var chapter = book.Chapter[1] = new();
            var section = chapter.Section[1] = new();
            var scroll = section.Scroll[1] = new();
            scroll.Text = "Test A";

            model.Coords.Library = 10;
            model.Coords.Shelf = 9;
            model.Coords.Series = 8;
            model.Coords.Collection = 7;
            model.Coords.Volume = 6;
            model.Coords.Book = 5;
            model.Coords.Chapter = 4;
            model.Coords.Section = 3;
            model.Coords.Scroll = 2;
            model.Terse.Scroll.Text = "Test B";

            var result = model.Serialize();
            var other = new TerseNotepad.TerseModel();
            other.Load(result, false);
            Assert.Equal(2, other.LeafCount);
            Assert.Equal(4, other.WordCount);

            Assert.NotNull(other.Terse.Root);
            if (other.Terse.Root != null)
            {
                Assert.Equal(2, other.Terse.Root.Library.Count);

                Assert.Single(other.Terse.Root.Library[1].Shelf);
                Assert.Single(other.Terse.Root.Library[1].Shelf[1].Series);
                Assert.Single(other.Terse.Root.Library[1].Shelf[1].Series[1].Collection);
                Assert.Single(other.Terse.Root.Library[1].Shelf[1].Series[1].Collection[1].Volume);
                Assert.Single(other.Terse.Root.Library[1].Shelf[1].Series[1].Collection[1].Volume[1].Book);
                Assert.Single(other.Terse.Root.Library[1].Shelf[1].Series[1].Collection[1].Volume[1].Book[1].Chapter);
                Assert.Single(other.Terse.Root.Library[1].Shelf[1].Series[1].Collection[1].Volume[1].Book[1].Chapter[1].Section);
                Assert.Single(other.Terse.Root.Library[1].Shelf[1].Series[1].Collection[1].Volume[1].Book[1].Chapter[1].Section[1].Scroll);
                Assert.Equal("Test A", other.Terse.Root.Library[1].Shelf[1].Series[1].Collection[1].Volume[1].Book[1].Chapter[1].Section[1].Scroll[1].Text);
            }

            other.Coords.Library = 10;
            other.Coords.Shelf = 9;
            other.Coords.Series = 8;
            Assert.Equal("Test B", other.Terse.Series.Collection[7].Volume[6].Book[5].Chapter[4].Section[3].Scroll[2].Text);
        }

        [Fact]
        public void Test_SparseFile()
        {
            // 50-17-13-9-215-44-42-13-6
            var model = new TerseNotepad.TerseModel();
            model.Coords.Library = 50;
            model.Coords.Shelf = 17;
            model.Coords.Series = 13;
            model.Coords.Volume = 9;
            model.Coords.Collection = 215;
            model.Coords.Book = 44;
            model.Coords.Chapter = 42;
            model.Coords.Section = 13;
            model.Coords.Scroll = 6;
            model.Terse.Scroll.Text = "Sparse File Test\n\n\nWith multiple lines.";
            var test = model.Serialize();
            var result = new TerseModel();
            result.Load(test, false);
            result.Coords = model.Coords;
            Assert.Equal(model.Terse.Scroll.Text, result.Terse.Scroll.Text);
        }

        [Fact]
        public void Test_Serialize_Known_Offsets()
        {
            var model = new TerseNotepad.TerseModel();
            var pattern1 = "1-1-1 AAA";
            var pattern2 = "50-33-22 ZZZ";
            model.Terse.Root.Library = new();
            model.Coords = new TerseNotepad.Coordinates(true);
            model.Terse.Scroll.Text = pattern1;
            model.Coords.Chapter = 50;
            model.Coords.Section = 33;
            model.Coords.Scroll = 22;
            model.Terse.Scroll.Text = pattern2;
            var serialized = model.Serialize();
            var result1 = serialized.Substring(0, pattern1.Length);
            var offset = 49 + 32 + 21 + pattern1.Length;
            var result2 = serialized.Substring(offset, pattern2.Length);
            Assert.Equal(pattern1, result1);
            Assert.Equal(pattern2, result2);
        }
    }
}
#pragma warning restore CA1416 // Validate platform compatibility