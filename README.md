# SystemPrograming_Exam
Create an application that allows searching for a specific set of prohibited words in files. The application's user interface should allow entering or loading a set of prohibited words from a file. By clicking the "Start" button, the application should begin searching for these words on all available storage devices (hard drives, flash drives).

Files containing prohibited words should be copied to a specified folder. In addition to the original file, create a new file with the content of the original file, where prohibited words are replaced with 7 repeated asterisks (*******).

Also, create a report file. It should contain information about all found files with prohibited words, their paths, file sizes, information about the number of replacements, etc. In the report file, also display the Top 10 most popular prohibited words.

The program's interface should display the progress of the application's work using indicators (progress bars). Through the application interface, the user can pause, resume, or completely stop the algorithm's operation.

After the program finishes its work, display the results in user interface elements (think about which control elements may be needed).

The program must use mechanisms of multi-threading and synchronization!

The program can only be run in a single instance.
