using System;
using System.Collections.Generic;
using System.IO;
using System.Transactions;
using VirtoCommerce.Platform.Core.FileManager.Operations;

namespace VirtoCommerce.Platform.Core.FileManager
{
    /// <summary>
    /// port from https://github.com/rsevil/Transactions
    /// </summary>
    public class FileManager : IFileManager
    {
        /// <summary>Creates all directories in the specified path.</summary>
        /// <param name="path">The directory path to create.</param>
        public void CreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }

            if (IsInTransaction())
                EnlistOperation(new CreateDirectory(path));
            else
                Directory.CreateDirectory(path);
        }

        public void Delete(string path)
        {
            FileAttributes attributes = File.GetAttributes(path);

            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                if (IsInTransaction())
                    EnlistOperation(new DeleteDirectory(path));
                else
                    Directory.Delete(path, true);
            }
            else
            {
                if (IsInTransaction())
                    EnlistOperation(new DeleteFile(path));
                else
                    File.Delete(path);
            }
        }

        public void SafeDelete(string path)
        {
            if (Directory.Exists(path))
            {
                //try delete whole directory
                try
                {
                    Delete(path);
                }
                //Because some folder can be locked by ASP.NET Bundles file monitor we should ignore IOException
                catch (IOException)
                {
                    //If fail need to delete directory content first
                    //Files                 
                    foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
                    {
                        Delete(file);
                    }
                    //Dirs
                    foreach (var subDirectory in Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            Delete(subDirectory);
                        }
                        catch (IOException)
                        {
                        }
                    }
                    //Then try to delete main directory itself
                    try
                    {
                        //_txFileManager.DeleteDirectory(directoryPath);
                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }


        /// <summary>Dictionary of transaction enlistment objects for the current thread.</summary>
        [ThreadStatic]
        private static Dictionary<string, FileManagerEnlistment> _enlistments;

        private static readonly object _enlistmentsLock = new object();

        private static bool IsInTransaction()
        {
            return Transaction.Current != null;
        }

        private static void EnlistOperation(IRollbackableOperation operation)
        {
            Transaction transaction = Transaction.Current;
            FileManagerEnlistment enlistment;

            lock (_enlistmentsLock)
            {
                if (_enlistments == null)
                {
                    _enlistments = new Dictionary<string, FileManagerEnlistment>();
                }

                if (!_enlistments.TryGetValue(transaction.TransactionInformation.LocalIdentifier, out enlistment))
                {
                    enlistment = new FileManagerEnlistment(transaction);
                    _enlistments.Add(transaction.TransactionInformation.LocalIdentifier, enlistment);
                }
                enlistment.EnlistOperation(operation);
            }
        }
    }
}
