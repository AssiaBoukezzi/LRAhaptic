/// ----------------------------///
///     RICCA Aylen 2016        ///
///     aricca8@gmail.com       ///
///     Internship IBISC        ///
/// ----------------------------///

//---------------------------------------------------------------------------
// INCLUDES
//---------------------------------------------------------------------------
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

//---------------------------------------------------------------------------
// PRODUCER-CONSUMER QUEUE
//---------------------------------------------------------------------------

/// <summary>
/// Thread Queue to log into files
/// </summary>
class ProducerConsumerQueue : IDisposable
{
    /// <summary>
    /// Queue element
    /// </summary>
    private struct LogEntry
    {
        /// <summary>
        /// File Descriptor
        /// </summary>
        public int fd;

        /// <summary>
        /// Delta time
        /// </summary>
        public double dTime;

        /// <summary>
        /// Haptic position
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// Struct constructor
        /// </summary>
        /// <param name="d">File descriptor</param>
        /// <param name="t">Delta time</param>
        /// <param name="p">Actual position</param>
        public LogEntry(int d, double t, Vector3 p)
        {
            dTime = t;
            fd = d;
            position = new Vector3(p.x, p.y, p.z);
        }
    }

    /// <summary>
    /// Flag indicating active state of thread
    /// </summary>
    private bool disposed = false;

    /// <summary>
    /// Function to determine if object was disposed
    /// </summary>
    public bool IsDisposed
    {
        get { return disposed; }
    }

    /// <summary>
    /// Handle to block thread
    /// </summary>
    private EventWaitHandle _wh = new AutoResetEvent(false);

    /// <summary>
    /// Thread who writes entries in Log files
    /// </summary>
    private Thread _worker;

    /// <summary>
    /// Locker for using the queue
    /// </summary>
    private readonly object _locker = new object();

    /// <summary>
    /// Queue of log entries to consume
    /// </summary>
    /// <remarks>nullable accepted</remarks>
    private Queue<LogEntry?> _tasksToLog = new Queue<LogEntry?>();

    /// <summary>
    /// Logger object for IO operations
    /// </summary>
    private XMLManager _logger;

    /// <summary>
    /// File descriptor associated with the logging file
    /// </summary>
    private int _fileDescriptor = -1;

    /// <summary>
    /// Initialization of the Producer-Consumer Queue
    /// </summary>
    /// <param name="Logger">Logger class for task execution</param>
    /// <param name="active">wether the trial must be logged or not</param>
    public ProducerConsumerQueue(XMLManager Logger, bool active, int fd)
    {
        _fileDescriptor = fd;

        if (_tasksToLog == null)
            _tasksToLog = new Queue<LogEntry?>();

        _logger = Logger;
        if (active)
            _worker = new Thread(Work);
        else
            _worker = new Thread(Skip);
        _worker.Start();
    }

    /// <summary>
    /// Inserts a new entry to log into the queue
    /// </summary>
    /// <param name="fd">File descriptor</param>
    /// <param name="t">Time elapsed since last frame [ms]</param>
    /// <param name="p">Position of the haptic</param>
    public void EnqueueTask(int fd, double t, Vector3 p)
    {
        LogEntry e = new LogEntry(fd, t, p);
        lock (_locker) _tasksToLog.Enqueue(e);
        _wh.Set();
    }

    /// <summary>
    /// Send a null task to end thread
    /// </summary>
    public void EnqueueTask()
    {
        lock (_locker) _tasksToLog.Enqueue(null);
        _wh.Set();
    }

    /// <summary>
    /// Destroys the thread created
    /// </summary>
    public void Dispose()
    {
        // Signal the consumer to exit
        EnqueueTask();
        // Wait for the consumer's thread to finish.
        _worker.Join();
        // Release any OS resources.
        _wh.Close();

        // Clean task queue
        _tasksToLog.Clear();

        disposed = true;
    }

    /// <summary>
    /// Destroys the thread created without finishing logging
    /// </summary>
    public void Delete()
    {
        // Wait for the consumer's thread to finish.
        _worker.Abort();
        // Release any OS resources.
        _wh.Close();

        // Clean task queue
        _tasksToLog.Clear();

        disposed = true;
    }

    /// <summary>
    /// Thread skip function
    /// </summary>
    void Skip()
    {
        while (true)
        {
            LogEntry? task = null;

            // Get a task
            lock (_locker)
                if (_tasksToLog.Count > 0)
                {
                    task = _tasksToLog.Dequeue();

                    // null task means finish thread
                    if (task == null) return;
                }

            //wait for a signal
            if (task == null) _wh.WaitOne();
        }
    }

    /// <summary>
    /// Thread start function
    /// </summary>
    void Work()
    {
        while (true)
        {
            LogEntry? task = null;

            // Get a task
            lock (_locker)
                if (_tasksToLog.Count > 0)
                {
                    task = _tasksToLog.Dequeue();

                    // null task means finish thread
                    if (task == null)
                    {
                        _logger.logEntry(_fileDescriptor, -999, new Vector3(-999, -999, -999));
                        _logger.closeFileEntry(_fileDescriptor);
                        return;
                    }
                }

            if (task != null)
            {
                // Task execution
                LogEntry entry = (LogEntry)task;
                _logger.logEntry(entry.fd, entry.dTime, entry.position);
            }
            else
                // No more tasks - wait for a signal
                _wh.WaitOne();
        }
    }
}