using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Windows;
using JetBrains.Annotations;
using OpenMLTD.ManifestTools.Web.TDAssets;
using OpenMLTD.MiriTore;

namespace OpenMLTD.ManifestTools.UI {
    partial class FAssetDownload {

        private sealed class WorkerThread {

            public WorkerThread([NotNull] FAssetDownload form, [NotNull] DownloadConfig downloadConfig, [NotNull] string saveDir) {
                _form = form;
                _downloadConfig = downloadConfig;
                _saveDir = saveDir;

                _thread = new Thread(DoWork);
                _thread.Name = "Asset Download Worker";
                _thread.IsBackground = true;
            }

            public void Start() {
                if (_thread.IsAlive) {
                    return;
                }

                _shouldStop = false;

                _thread.Start();
            }

            public void Abort() {
                if (!_thread.IsAlive) {
                    return;
                }

                _shouldStop = true;

                _thread.Join();
            }

            private async void DoWork() {
                var items = _form._items;
                var jobCount = items.Length;
                var downloaded = _form._downloaded;

                for (var i = 0; i < jobCount; i += 1) {
                    if (downloaded[i]) {
                        continue;
                    }

                    if (_shouldStop) {
                        return;
                    }

                    var localName = items[i].LocalName;
                    var remoteName = items[i].RemoteName;
                    var dstPath = Path.Combine(_saveDir, localName);

                    try {
                        await TDDownloader.DownloadToFile(_downloadConfig.ResourceVersion, remoteName, dstPath, _downloadConfig.UnityVersion);
                    } catch (HttpRequestException ex) {
                        var message = $"Failed to download {localName}. Please check download configuration (probably a res version mismatch), or Internet connection.";
                        AlertInMainThread(new ApplicationException(message, ex));
                        return;
                    } catch (Exception ex) {
                        AlertInMainThread(ex);
                        return;
                    }

                    downloaded[i] = true;

                    _form.ProgressFile(i, jobCount, localName);
                }

                _form.OnAllDownloadsComplete();
            }

            private void AlertInMainThread([NotNull] Exception ex) {
                _form.Invoke(new Action<Exception>(e => {
                    MessageBox.Show(e.ToString(), ApplicationHelper.GetApplicationTitle(), MessageBoxButton.OK, MessageBoxImage.Error);
                }), ex);
            }

            [NotNull]
            private readonly Thread _thread;

            private bool _shouldStop;

            [NotNull]
            private readonly FAssetDownload _form;

            [NotNull]
            private DownloadConfig _downloadConfig;

            [NotNull]
            private string _saveDir;

        }

    }
}
