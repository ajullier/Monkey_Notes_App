namespace NoteWebMVC.Utilities
{
    public class ReadUrlConfiguration
    {
        public string Url { get; set; } = string.Empty;

        public ReadUrlConfiguration() {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.

                using (StreamReader sr = new StreamReader("Url.txt"))
                {

                    // Read and display lines from the file until the end of
                    // the file is reached.
                    Url = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

        }

    }
}
