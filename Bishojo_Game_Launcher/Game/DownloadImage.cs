﻿using BishojoGameLauncher.Database;
using BishojoGameLauncher.Properties;
using MihaZupan;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BishojoGameLauncher.Game {
	class DownloadImage {
        public DownloadImage() {
            webClient = new WebClient();
		}

        private WebClient webClient;

        public void UseHTTPProxy(string address, int port) {
            webClient.Proxy = new WebProxy($"{address}:{port}");
        }

        public void UseHTTPProxy(string address, int port, string account, string password) {
            webClient.Proxy = new WebProxy($"{address}:{port}");
            webClient.Proxy.Credentials = new NetworkCredential(account, password);
        }

        public void UseSocks5Proxy(string address, int port) {
            webClient.Proxy = new HttpToSocks5Proxy(address, port);
        }

        public void UseSocks5Proxy(string address, int port, string account, string password) {
            webClient.Proxy = new HttpToSocks5Proxy(address, port, account, password);
        }

        public async Task Run(GameData.GamesRow detaile) {
            try {
                await Task.Run(() => {
                    AppFolder.ExistsGameFolder(detaile.Hash);
                    string url;

                    //MainImage Download
                    if (detaile.MainImage.StartsWith("http", StringComparison.OrdinalIgnoreCase)) {
                        url = detaile.MainImage;
                    } else {
                        url = "http:" + detaile.MainImage;
                    }
                    webClient.DownloadFile(
                        url,
                        AppPath.GamesFolder +
                        detaile.Hash + @"\" +
                        detaile.Hash + Path.GetExtension(detaile.MainImage)
                    );

                    //SampleCG Download
                    var count = 0;
                    foreach (var sampleCG in detaile.SampleCGs) {
                        if (sampleCG.StartsWith("http", StringComparison.OrdinalIgnoreCase)) {
                            url = sampleCG;
                        } else {
                            url = "http:" + sampleCG;
                        }
                        webClient.DownloadFile(
                            url,
                            AppPath.GamesFolder +
                            detaile.Hash + @"\" +
                            @"sample\" +
                            "sample" + count.ToString() + Path.GetExtension(detaile.MainImage)
                        );
                        count++;
                    }
                    webClient.Dispose();
                });
            }
            catch {
                throw;
            }
        }

	}
}
