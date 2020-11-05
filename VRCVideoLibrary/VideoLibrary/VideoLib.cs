﻿using MelonLoader;
using RubyButtonAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIExpansionKit.API;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.SDK3.Video.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDK3.Video.Interfaces;
using VRC.SDK3.Video.Interfaces.AVPro;
using VRC.SDKBase;
using VRCSDK2;

namespace VideoLibrary
{
    public static class LibraryBuildInfo
    {
        public const string modName = "VRC Video Library";
        public const string modVersion = "0.6.2";
        public const string modAuthor = "UHModz";
        public const string modDownload = "https://github.com/UshioHiko/VRCVideoLibrary/releases";
    }

    public class VideoLib : MelonMod
    {
        protected List<ModVideo> videoList;
 
        private int indexNumber = 0;
        private int currentMenuIndex;
        private bool onCooldown = false;
        private bool getLink = false;

        private string videoDirectory;

        private QMNestedButton videoLibrary;

        private QMSingleButton previousButton;
        private QMSingleButton nextButton;

        public override void OnApplicationStart()
        {
            videoList = new List<ModVideo>();
            InitializeLibrary();
        }

        public override void VRChat_OnUiManagerInit()
        {
            videoLibrary = new QMNestedButton("ShortcutMenu", -10, 0, "", "", null, null, null, null);
            videoLibrary.getMainButton().getGameObject().GetComponentInChildren<Image>().enabled = false;
            videoLibrary.getMainButton().getGameObject().GetComponentInChildren<Text>().enabled = false;

            ExpansionKitApi.RegisterSimpleMenuButton(ExpandedMenu.QuickMenu, "Video\nLibrary", delegate
            {
                videoLibrary.getMainButton().getGameObject().GetComponent<Button>().Press();
            });

            var indexButton = new QMSingleButton(videoLibrary, 4, 1, "Page:\n" + (currentMenuIndex + 1).ToString() + " of " + (indexNumber + 1).ToString(), delegate { }, "", Color.clear, Color.yellow);
            indexButton.getGameObject().GetComponentInChildren<Button>().enabled = false;
            indexButton.getGameObject().GetComponentInChildren<Image>().enabled = false;

            previousButton = new QMSingleButton(videoLibrary, 4, 0, "Previous\nPage", delegate
            {
                if (currentMenuIndex != 0)
                {
                    currentMenuIndex--;
                }

                foreach (ModVideo videoButton in videoList)
                {
                    if (videoButton.IndexNumber != currentMenuIndex)
                    {
                        videoButton.VideoButton.setActive(false);
                    }

                    else
                    {
                        videoButton.VideoButton.setActive(true);
                    }
                }
                indexButton.setButtonText("Page:\n" + (currentMenuIndex + 1).ToString() + " of " + (indexNumber + 1).ToString());
            }, "Previous video page", null, null);

            nextButton = new QMSingleButton(videoLibrary, 4, 2, "Next\nPage", delegate
            {
                if (currentMenuIndex != indexNumber)
                {
                    currentMenuIndex++;
                }

                foreach (ModVideo videoButton in videoList)
                {
                    if (videoButton.IndexNumber != currentMenuIndex)
                    {
                        videoButton.VideoButton.setActive(false);
                    }

                    else
                    {
                        videoButton.VideoButton.setActive(true);
                    }
                }
                indexButton.setButtonText("Page:\n" + (currentMenuIndex + 1).ToString() + " of " + (indexNumber + 1).ToString());
            }, "Previous video page", null, null);

            var openReadMe = new QMSingleButton(videoLibrary, 5, 0, "Read\nMe", delegate
            {
                Process.Start("https://github.com/UshioHiko/VRCVideoLibrary/blob/master/README.md");
            }, "Opens a link to the mod's \"Read Me\"");

            var openListButton = new QMSingleButton(videoLibrary, 5, -1, "Open\nLibrary\nDocument", delegate
            {
                OpenVideoLibrary();
            }, "Opens the Video Library text document\nLibrary Format: \"Button Name|Video Url\"", null, null);

            var getLinkToggle = new QMToggleButton(videoLibrary, 5, 1, "Buttons Copy\nVideo Link", delegate
            {
                getLink = true;
            }, "Disabled", delegate
            {
                getLink = false;
            }, "Makes video library buttons copy video url to your system clipboard", null, null, false, false);

            var videoFromClipboard = new QMSingleButton(videoLibrary, 0, -1, "Video From\nClipboard", delegate
            {
                MelonCoroutines.Start(ModVideo.VideoFromClipboard(onCooldown));
            }, "Puts the link in your system clipboard into the world's video player");

            foreach (ModVideo video in videoList)
            {
                switch (video.VideoNumber)
                {
                    case 0:
                        {
                            var vidButton = new QMSingleButton(videoLibrary, 1, 0, video.VideoName, delegate
                            {
                                if (getLink)
                                {
                                    video.GetLink();
                                }

                                else
                                {
                                    MelonCoroutines.Start(video.AddVideo(onCooldown));

                                    if (!onCooldown)
                                    {
                                        MelonCoroutines.Start(CoolDown());
                                    }
                                }
                            }, $"Puts {video.VideoName} on the video player", null, null);

                            video.VideoButton = vidButton;
                            vidButton.getGameObject().GetComponentInChildren<Text>().resizeTextForBestFit = true;
                            break;
                        }

                    case 1:
                        {
                            var vidButton = new QMSingleButton(videoLibrary, 2, 0, video.VideoName, delegate
                            {
                                if (getLink)
                                {
                                    video.GetLink();
                                }

                                else
                                {
                                    MelonCoroutines.Start(video.AddVideo(onCooldown));

                                    if (!onCooldown)
                                    {
                                        MelonCoroutines.Start(CoolDown());
                                    }
                                }
                            }, $"Puts {video.VideoName} on the video player", null, null);

                            video.VideoButton = vidButton;
                            vidButton.getGameObject().GetComponentInChildren<Text>().resizeTextForBestFit = true;
                            break;
                        }

                    case 2:
                        {
                            var vidButton = new QMSingleButton(videoLibrary, 3, 0, video.VideoName, delegate
                            {
                                if (getLink)
                                {
                                    video.GetLink();
                                }

                                else
                                {
                                    MelonCoroutines.Start(video.AddVideo(onCooldown));

                                    if (!onCooldown)
                                    {
                                        MelonCoroutines.Start(CoolDown());
                                    }
                                }
                            }, $"Puts {video.VideoName} on the video player", null, null);

                            video.VideoButton = vidButton;
                            vidButton.getGameObject().GetComponentInChildren<Text>().resizeTextForBestFit = true;
                            break;
                        }

                    case 3:
                        {
                            var vidButton = new QMSingleButton(videoLibrary, 1, 1, video.VideoName, delegate
                            {
                                if (getLink)
                                {
                                    video.GetLink();
                                }

                                else
                                {
                                    MelonCoroutines.Start(video.AddVideo(onCooldown));

                                    if (!onCooldown)
                                    {
                                        MelonCoroutines.Start(CoolDown());
                                    }
                                }
                            }, $"Puts {video.VideoName} on the video player", null, null);

                            video.VideoButton = vidButton;
                            vidButton.getGameObject().GetComponentInChildren<Text>().resizeTextForBestFit = true;
                            break;
                        }

                    case 4:
                        {
                            var vidButton = new QMSingleButton(videoLibrary, 2, 1, video.VideoName, delegate
                            {
                                if (getLink)
                                {
                                    video.GetLink();
                                }

                                else
                                {
                                    MelonCoroutines.Start(video.AddVideo(onCooldown));

                                    if (!onCooldown)
                                    {
                                        MelonCoroutines.Start(CoolDown());
                                    }
                                }
                            }, $"Puts {video.VideoName} on the video player", null, null);

                            video.VideoButton = vidButton;
                            vidButton.getGameObject().GetComponentInChildren<Text>().resizeTextForBestFit = true;
                            break;
                        }

                    case 5:
                        {
                            var vidButton = new QMSingleButton(videoLibrary, 3, 1, video.VideoName, delegate
                            {
                                if (getLink)
                                {
                                    video.GetLink();
                                }

                                else
                                {
                                    MelonCoroutines.Start(video.AddVideo(onCooldown));

                                    if (!onCooldown)
                                    {
                                        MelonCoroutines.Start(CoolDown());
                                    }
                                }
                            }, $"Puts {video.VideoName} on the video player", null, null);

                            video.VideoButton = vidButton;
                            vidButton.getGameObject().GetComponentInChildren<Text>().resizeTextForBestFit = true;
                            break;
                        }

                    case 6:
                        {
                            var vidButton = new QMSingleButton(videoLibrary, 1, 2, video.VideoName, delegate
                            {
                                if (getLink)
                                {
                                    video.GetLink();
                                }

                                else
                                {
                                    MelonCoroutines.Start(video.AddVideo(onCooldown));

                                    if (!onCooldown)
                                    {
                                        MelonCoroutines.Start(CoolDown());
                                    }
                                }
                            }, $"Puts {video.VideoName} on the video player", null, null);

                            video.VideoButton = vidButton;
                            vidButton.getGameObject().GetComponentInChildren<Text>().resizeTextForBestFit = true;
                            break;
                        }

                    case 7:
                        {
                            var vidButton = new QMSingleButton(videoLibrary, 2, 2, video.VideoName, delegate
                            {
                                if (getLink)
                                {
                                    video.GetLink();
                                }

                                else
                                {
                                    MelonCoroutines.Start(video.AddVideo(onCooldown));

                                    if (!onCooldown)
                                    {
                                        MelonCoroutines.Start(CoolDown());
                                    }
                                }
                            }, $"Puts {video.VideoName} on the video player", null, null);

                            video.VideoButton = vidButton;
                            vidButton.getGameObject().GetComponentInChildren<Text>().resizeTextForBestFit = true;
                            break;
                        }

                    case 8:
                        {
                            var vidButton = new QMSingleButton(videoLibrary, 3, 2, video.VideoName, delegate
                            {
                                if (getLink)
                                {
                                    video.GetLink();
                                }

                                else
                                {
                                    MelonCoroutines.Start(video.AddVideo(onCooldown));

                                    if (!onCooldown)
                                    {
                                        MelonCoroutines.Start(CoolDown());
                                    }
                                }
                            }, $"Puts {video.VideoName} on the video player", null, null);

                            video.VideoButton = vidButton;
                            vidButton.getGameObject().GetComponentInChildren<Text>().resizeTextForBestFit = true;
                            break;
                        }
                }

                if (video.IndexNumber != currentMenuIndex)
                {
                    video.VideoButton.setActive(false);
                }
            }

            if (videoList.Count <= 9)
            {
                previousButton.setIntractable(false);
                nextButton.setIntractable(false);
            }
        }

        public void InitializeLibrary()
        {
            string exampleVideo = "Example Name|https://youtu.be/pKO9UjSeLew";

            var rootDirectory = Application.dataPath;
            rootDirectory += @"\..\";

            var subDirectory = rootDirectory + @"\UHModz\";

            videoDirectory = subDirectory + "Videos.txt";

            if (!Directory.Exists(subDirectory))
            {
                Directory.CreateDirectory(subDirectory);
                MelonLogger.Log("Created UHModz Directory!");
            }

            if (!File.Exists(videoDirectory))
            {
                using (StreamWriter sw = File.CreateText(videoDirectory))
                {
                    sw.WriteLine(exampleVideo);
                    sw.Close();
                }
            }

            GetVideoLibrary();
        }

        public void GetVideoLibrary()
        {
            indexNumber = 0;
            currentMenuIndex = 0;
            StreamReader file = new StreamReader(videoDirectory);


            string line;
            while ((line = file.ReadLine()) != null)
            {
                var lineArray = line.Split('|');
                videoList.Add(new ModVideo(lineArray[0], lineArray[1])); //{ VideoName = lineArray[0], VideoLink = lineArray[1] }
            }

            file.Close();

            videoList.Sort();

            var videoNumber = 0;

            for (int i = 0; i < videoList.Count; i++)
            {
                var video = videoList[i];

                video.VideoNumber = videoNumber;
                video.IndexNumber = indexNumber;

                videoNumber++;
                if (videoNumber == 9 && i != (videoList.Count - 1))
                {
                    indexNumber++;
                    videoNumber = 0;
                }

                else
                {
                    continue;
                }
            }
        }

        public void OpenVideoLibrary()
        {
            Process.Start(videoDirectory);
        }

        public IEnumerator CoolDown()
        {
            onCooldown = true;
            yield return new WaitForSeconds(30);
            onCooldown = false;
        }
    }

    public class ModVideo : IComparable<ModVideo>
    {
        public ModVideo(string videoName, string videoLink, int videoNumber = 0, int indexNumber = 0)
        {
            this.VideoName = videoName;
            this.VideoLink = videoLink;
            this.VideoNumber = videoNumber;
            this.IndexNumber = indexNumber;
        }

        public string VideoName { get; set; }
        public string VideoLink { get; set; }
        public int VideoNumber { get; set; }
        public int IndexNumber { get; set; }
        public QMSingleButton VideoButton { get; set; }

        public int CompareTo(ModVideo other)
        {
            return this.VideoName.CompareTo(other.VideoName);
        }

        public void DestroyButton()
        {
            VideoButton.DestroyMe();
        }

        public IEnumerator AddVideo(bool onCooldown)
        {
            var videoPlayerActive = VideoPlayerCheck();
            var isMaster = MasterCheck(APIUser.CurrentUser.id);

            if (videoPlayerActive)
            {
                if (isMaster)
                {
                    if (!onCooldown)
                    {
                        var videoPlayer = GameObject.FindObjectOfType<VRC_SyncVideoPlayer>();
                        var udonPlayer = GameObject.FindObjectOfType<VRCUnityVideoPlayer>();

                        VideoPlayerType playerType = VideoPlayerType.None;

                        if (videoPlayer != null) playerType = VideoPlayerType.ClassicPlayer;
                        else if (udonPlayer != null) playerType = VideoPlayerType.UdonPlayer;


                        if (playerType == VideoPlayerType.ClassicPlayer)
                        {
                            videoPlayer.Clear();
                            videoPlayer.AddURL(VideoLink);

                            VRCUiManager.prop_VRCUiManager_0.field_Private_List_1_String_0.Add("Wait 30 seconds\nfor video to play");

                            yield return new WaitForSeconds(30);

                            videoPlayer.Next();
                        }

                        else if (playerType == VideoPlayerType.UdonPlayer)
                        {
                            udonPlayer.videoURL.url = VideoLink;

                            VRCUiManager.prop_VRCUiManager_0.field_Private_List_1_String_0.Add("Wait 30 seconds\nfor video to play");

                            yield return new WaitForSeconds(30);

                            udonPlayer.LoadURL(udonPlayer.videoURL);
                        }
                    }

                    else
                    {
                        VRCUiManager.prop_VRCUiManager_0.field_Private_List_1_String_0.Add("Video Library is on 30 second cooldown");
                    }
                }

                else
                {
                    VRCUiManager.prop_VRCUiManager_0.field_Private_List_1_String_0.Add("Only the master can set videos...");
                }
            }
        }

        public void GetLink()
        {
            System.Windows.Forms.Clipboard.SetText(VideoLink);
            VRCUiManager.prop_VRCUiManager_0.field_Private_List_1_String_0.Add("Video link copied to system clipboard");
        }

        private static bool MasterCheck(string UserID)
        {
            bool isMaster = false;
            var playerList = PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;

            foreach (Player player in playerList)
            {
                var playerApi = player.prop_VRCPlayerApi_0;
                var apiUser = player.field_Private_APIUser_0;

                if (playerApi.isMaster)
                {
                    if (apiUser.id == UserID)
                    {
                        isMaster = true;
                        break;
                    }

                    else
                    {
                        isMaster = false;
                        break;
                    }
                }
            }

            return isMaster;
        }

        public static IEnumerator VideoFromClipboard(bool onCooldown)
        {
            var videoPlayerActive = VideoPlayerCheck();
            var isMaster = MasterCheck(APIUser.CurrentUser.id);

            if (videoPlayerActive)
            {
                if (isMaster)
                {
                    if (!onCooldown)
                    {
                        var videoPlayer = GameObject.FindObjectOfType<VRC_SyncVideoPlayer>();
                        var udonPlayer = GameObject.FindObjectOfType<VRCUnityVideoPlayer>();

                        VideoPlayerType playerType = VideoPlayerType.None;

                        if (videoPlayer != null) playerType = VideoPlayerType.ClassicPlayer;
                        else if (udonPlayer != null) playerType = VideoPlayerType.UdonPlayer;

                        if (playerType == VideoPlayerType.ClassicPlayer)
                        {
                            videoPlayer.Clear();
                            videoPlayer.AddURL(System.Windows.Forms.Clipboard.GetText());

                            VRCUiManager.prop_VRCUiManager_0.field_Private_List_1_String_0.Add("Wait 30 seconds\nfor video to play");

                            yield return new WaitForSeconds(30);

                            videoPlayer.Next();
                        }

                        else if (playerType == VideoPlayerType.UdonPlayer)
                        {
                            udonPlayer.videoURL.url = System.Windows.Forms.Clipboard.GetText();

                            VRCUiManager.prop_VRCUiManager_0.field_Private_List_1_String_0.Add("Wait 30 seconds\nfor video to play");

                            yield return new WaitForSeconds(30);

                            udonPlayer.LoadURL(udonPlayer.videoURL);
                        }
                    }

                    else
                    {
                        VRCUiManager.prop_VRCUiManager_0.field_Private_List_1_String_0.Add("Video Library is on 30 second cooldown");
                    }
                }

                else
                {
                    VRCUiManager.prop_VRCUiManager_0.field_Private_List_1_String_0.Add("Only the master can set videos...");
                }
            }
        }

        private static bool VideoPlayerCheck()
        {
            bool videoPlayerActive;
            try
            {
                var videoPlayer = GameObject.FindObjectOfType<VRC_SyncVideoPlayer>();
                var udonPlayer = GameObject.FindObjectOfType<VRCUnityVideoPlayer>();

                if (videoPlayer != null || udonPlayer != null)
                {
                    videoPlayerActive = true;
                    return videoPlayerActive;
                }

                else
                {
                    videoPlayerActive = false;
                    return videoPlayerActive;
                }
            }

            catch (Exception)
            {
                videoPlayerActive = false;
                return videoPlayerActive;
            }
        }

        public enum VideoPlayerType
        {
            UdonPlayer,
            ClassicPlayer,
            None
        }
    }
}
