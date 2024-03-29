﻿/* Program.cs --------------------------------
 * 
 * * Copyright (c) 2009 Tigran Martirosyan 
 * * Contact and Information: tigranmt@gmail.com 
 * * This application is free software; you can redistribute it and/or 
 * * Modify it under the terms of the GPL license
 * * THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, 
 * * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
 * * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
 * * OTHER DEALINGS IN THE SOFTWARE. 
 * * THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE 
 *  */
// ---------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SvnObjects.Objects;
using ConsoleApplicationSubStat.Base.Query;

namespace ConsoleApplicationSubStat
{

    public static class ProgramConfiguration
    {       
        public static FolderRepoInfo REPOSITORY_FOLDER = null;
    }

    class Program
    {
        static readonly string sDBFileName = "stat.db";
        internal static SvnObjects.SvnFunctions.SubversionFunctions subFunc = null;
      

        static void Main(string[] args)
        {

            if (args == null || args.Length < 2)
            {
                WriteError("Not enough arguments for running stat. Use -h to gte help on possible arguments.");
                Console.ReadLine();
                return;
            }

            //Subverion execurable path control
            if (!System.IO.File.Exists(args[0]))
            {
                WriteError("Not valid subversion executable path : " + args[0]);
                Console.ReadLine();
                return;
            }

            //Working directory control
            if (!System.IO.Directory.Exists(args[1]))
            {
                WriteError("Not valid or not existing subversion repository directory path : " + args[1]);
                Console.ReadLine();
                return;
            }

            string sSubversionExePath = args[0];
            string repository_local_path = args[1];


            ///Sample for query Subverion repository
            subFunc = new SvnObjects.SvnFunctions.SubversionFunctions(sSubversionExePath);

             //first update repository to latest version 
            bool updatesucceed = subFunc.UpdateRepository(repository_local_path);
            if (!updatesucceed)
            {
                WriteError("Failed update repository. Can not generate statistics on this repository. Please fix the problem and run the program again.");
                Console.ReadLine();
                return;
            }
            ProgramConfiguration.REPOSITORY_FOLDER = subFunc.GetFolderRepoInfo(repository_local_path, -1);
            List<RepositoryInfo> lRepository = subFunc.GetRepositoryLogImmediate(ProgramConfiguration.REPOSITORY_FOLDER, -1);
            
            //reverse collection to begin insert from lowest revision available
            lRepository.Reverse();

            QueryBase.AddRevisionsToDB(lRepository);

         
        }



        private static void WriteError(string sErroString)
        {
            WriteString(sErroString, ConsoleColor.Red);
        }


        private static void WriteWarning(string sWarning)
        {
            WriteString(sWarning, ConsoleColor.Yellow);
        }


        private static void WriteMessage(string sMessage)
        {
            WriteString(sMessage, ConsoleColor.White);
        }


        private static void WriteString(string sString, ConsoleColor color) 
        {
            ConsoleColor curColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(sString);
            Console.ForegroundColor = curColor;
        }
    }
}
