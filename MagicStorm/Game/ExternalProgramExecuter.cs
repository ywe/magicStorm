using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;


namespace MagicStorm.Game
{
    public enum ExternalProgramExecuteResult
    {
        Ok,                // все хорошо
        InternalError,     // ошибка в процессе выполнения внешней программы
        TimeOut,           // слишком длительное время выполнения внешней программы
        WriteInputError,   // ошибка записи входных данных для внешней программы
        NoOutput,          // отсутствует файл выходных данных
        ReadOutputError,   // ошибка в процессе чтения выходных данных
        EmptyOutput,       // пустой файл выходных данных
        NotStarted,        // не удалось запустить внешнюю программы
        WrongInputData,    // неверные входные данные (обрабатывается в наследниках ExternalProgramExecuter)
        WrongOutputFormat, // неверный формат выходных данных (обрабатывается в наследниках ExternalProgramExecuter)
        WrongOutputData,   // неверные выходные данные (обрабатывается в наследниках ExternalProgramExecuter)
        OtherError         // другая ошибка
    }


    public class ExternalProgramExecuter
    {
        /*
          Интервал, с которым опрашивается состояние запущенного внешнего процесса (в миллисекундах)
        */
        public const int ProcessCheckTimeInterval = 10;

        private string programExecutable;
        private string localDriteProgramDirectory;
        private string inputFileName,
                       outputFileName;


        public ExternalProgramExecuter(string programExecutable,
                                       string inputFileName, string outputFileName)
        {
            this.programExecutable = programExecutable;
            this.inputFileName = inputFileName;
            this.outputFileName = outputFileName;
            Random rnd = new Random();
            rnd = new Random(rnd.Next() + programExecutable.GetHashCode());
            string randomStr = "";
            for (int i = 0; i < 8; i++)
                randomStr += "0123456789ABCDEF"[rnd.Next(16)];
#if NET40
      localDriteProgramDirectory = Path.Combine(Path.GetTempPath(), TempSubdir, randomStr);
#else
            localDriteProgramDirectory = Path.Combine(Path.Combine(Path.GetTempPath(), TempSubdir), randomStr);
#endif
            Init();
        }


        public void Init()
        {
            DeleteLocalDriveProgramDirectory();
            try
            {
                Directory.CreateDirectory(LocalDriteProgramDirectory);
            }
            catch (Exception e)
            {
                throw new ExternalProgramExecuterException(string.Format("Error creating subdir ({0})", LocalDriteProgramDirectory), e);
            }
            try
            {
                File.Copy(ProgramExecutable, LocalDriveProgramExecutable);
            }
            catch (Exception e)
            {
                throw new ExternalProgramExecuterException(string.Format("Error coping file ({0}) into file ({1})", ProgramExecutable, LocalDriveProgramExecutable), e);
            }
        }


        public void DeleteLocalDriveProgramDirectory()
        {
            try
            {
                if (Directory.Exists(LocalDriteProgramDirectory))
                    Directory.Delete(LocalDriteProgramDirectory, true);
            }
            catch (Exception e)
            {
                throw new ExternalProgramExecuterException(string.Format("Error deleting subdir ({0})", LocalDriteProgramDirectory), e);
            }
        }


        public void DeleteTempSubdir()
        {
            try
            {
                Directory.Delete(TempSubdir, true);
            }
            catch (Exception e)
            {
                throw new ExternalProgramExecuterException(string.Format("Error deleting temp subdir ({0})", TempSubdir), e);
            }
        }


        /*
          Исполняет программу, подсовывая в качестве входных данных inputFileContent;
          воpащает код выполнения и через outputFileContent содержимое выходного файла;
          maxTime - максимальное время выполнения в секундах, после чего процесс убивается
        */
        public virtual ExternalProgramExecuteResult Execute(string inputFileContent, double maxTime,
                                                            out string outputFileContent, out string comment)
        {
            outputFileContent = null;
            comment = null;

            string inputFileName = Path.Combine(LocalDriteProgramDirectory, this.inputFileName),
                   outputFileName = Path.Combine(LocalDriteProgramDirectory, this.outputFileName);
            Process process = null;
            try
            {
                try
                {
                    File.WriteAllText(inputFileName, inputFileContent, Encoding.Default);
                }
                catch (Exception)
                {
                    return ExternalProgramExecuteResult.WriteInputError;
                }
                process = new Process();
                process.StartInfo.WorkingDirectory = LocalDriteProgramDirectory;
                process.StartInfo.FileName = LocalDriveProgramExecutable;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (process.Start())
                {
                    DateTime startTime = DateTime.Now;
                    while (!process.HasExited)
                    {
                        Thread.Sleep(ProcessCheckTimeInterval);
                        if ((DateTime.Now - startTime).TotalSeconds > maxTime)
                            break;
                    }
                    if (process.HasExited)
                    {
                        if (process.ExitCode == 0)
                        {
                            if (File.Exists(outputFileName))
                            {
                                try
                                {
                                    outputFileContent = File.ReadAllText(outputFileName, Encoding.Default);
                                    if (outputFileContent == null || outputFileContent.Trim() == "")
                                        return ExternalProgramExecuteResult.EmptyOutput;

                                    return ExternalProgramExecuteResult.Ok;
                                }
                                catch (Exception)
                                {
                                    return ExternalProgramExecuteResult.ReadOutputError;
                                }
                            }
                            else
                                return ExternalProgramExecuteResult.NoOutput;
                        }
                        else
                        {
                            comment = string.Format("ExitCode = {0}", process.ExitCode);
                            return ExternalProgramExecuteResult.InternalError;
                        }
                    }
                    else
                    {
                        try
                        {
                            process.Kill();
                            while (!process.HasExited)
                                Thread.Sleep(ProcessCheckTimeInterval);
                        }
                        catch (Exception)
                        {
                        }
                        return ExternalProgramExecuteResult.TimeOut;
                    }
                }
                else
                    return ExternalProgramExecuteResult.NotStarted;
            }
            //      catch(Exception) {
            finally
            {
                // еще одна попытка убить если вдруг работающий процесс
                try
                {
                    if (process != null && !process.HasExited)
                        process.Kill();
                }
                catch (Exception) { }

                //        throw;
            }
        }


        public virtual string TempSubdir { get { return "Temp1"; } }

        public string ProgramExecutable { get { return programExecutable; } }
        public string ProgramExecutableFilnameOnly { get { return Path.GetFileName(programExecutable); } }
        public string LocalDriteProgramDirectory { get { return localDriteProgramDirectory; } }
        public string LocalDriveProgramExecutable { get { return Path.Combine(LocalDriteProgramDirectory, ProgramExecutableFilnameOnly); } }
        public string InputFileName { get { return inputFileName; } }
        public string OutputFileName { get { return outputFileName; } }
    }


    public class ExternalProgramExecuterException : ApplicationException
    {
        public ExternalProgramExecuterException()
            : base()
        {
        }

        public ExternalProgramExecuterException(string message)
            : base(message)
        {
        }

        public ExternalProgramExecuterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
