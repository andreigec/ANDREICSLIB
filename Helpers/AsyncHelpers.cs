using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ANDREICSLIB.Helpers
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Backgrounder
    /// </summary>
    public static class AsyncHelpers
    {
        /// <summary>
        /// Runs the synchronize.
        /// </summary>
        /// <param name="task">The task.</param>
        public static void RunSync(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            synch.Post(async _ =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }

        /// <summary>
        /// Runs the synchronize.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            var ret = default(T);
            synch.Post(async _ =>
            {
                try
                {
                    ret = await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);
            return ret;
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private readonly Queue<Tuple<SendOrPostCallback, object>> items =
                new Queue<Tuple<SendOrPostCallback, object>>();

            private readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
            private bool done;
            public Exception InnerException { get; set; }

            /// <summary>
            /// Sends the specified d.
            /// </summary>
            /// <param name="d">The d.</param>
            /// <param name="state">The state.</param>
            /// <exception cref="NotSupportedException">We cannot send to our same thread</exception>
            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            /// <summary>
            /// Posts the specified d.
            /// </summary>
            /// <param name="d">The d.</param>
            /// <param name="state">The state.</param>
            public override void Post(SendOrPostCallback d, object state)
            {
                lock (items)
                {
                    items.Enqueue(Tuple.Create(d, state));
                }
                workItemsWaiting.Set();
            }

            /// <summary>
            /// Ends the message loop.
            /// </summary>
            public void EndMessageLoop()
            {
                Post(_ => done = true, null);
            }

            /// <summary>
            /// Begins the message loop.
            /// </summary>
            /// <exception cref="AggregateException">AsyncHelpers.Run method threw an exception.</exception>
            public void BeginMessageLoop()
            {
                while (!done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (items)
                    {
                        if (items.Count > 0)
                        {
                            task = items.Dequeue();
                        }
                    }
                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null) // the method threw an exeption
                        {
                            throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                        }
                    }
                    else
                    {
                        workItemsWaiting.WaitOne();
                    }
                }
            }

            /// <summary>
            /// When overridden in a derived class, creates a copy of the synchronization context.
            /// </summary>
            /// <returns>
            /// A new <see cref="T:System.Threading.SynchronizationContext" /> object.
            /// </returns>
            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
    }
}