﻿using Lively.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lively.Common.Helpers.MVVM;
using Lively.Common;
using Microsoft.UI.Xaml;
using Lively.Common.Helpers.Storage;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Lively.UI.WinUI.Helpers;
using CommunityToolkit.Mvvm.Input;

namespace Lively.UI.WinUI.ViewModels
{
    public class AddWallpaperDataViewModel : ObservableObject
    {
        private readonly ILibraryModel libData;

        public AddWallpaperDataViewModel(ILibraryModel obj)
        {
            this.libData = obj;

            //use existing data for editing already imported wallpaper..
            Title = libData.LivelyInfo.Title;
            Desc = libData.LivelyInfo.Desc;
            Url = libData.LivelyInfo.Contact;
            Author = libData.LivelyInfo.Author;
        }

        #region data

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = (value?.Length > 100 ? value.Substring(0, 100) : value);
                libData.Title = _title;
                libData.LivelyInfo.Title = _title;
                OnPropertyChanged();
            }
        }

        private string _desc;
        public string Desc
        {
            get { return _desc; }
            set
            {
                _desc = (value?.Length > 5000 ? value.Substring(0, 5000) : value);
                libData.Desc = _desc;
                libData.LivelyInfo.Desc = _desc;
                OnPropertyChanged();
            }
        }

        private string _author;
        public string Author
        {
            get { return _author; }
            set
            {
                _author = (value?.Length > 100 ? value.Substring(0, 100) : value);
                libData.Author = _author;
                libData.LivelyInfo.Author = _author;
                OnPropertyChanged();
            }
        }

        private string _url;
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                try
                {
                    libData.SrcWebsite = LinkHandler.SanitizeUrl(_url);
                }
                catch
                {
                    libData.SrcWebsite = null;
                }
                libData.LivelyInfo.Contact = _url;
                OnPropertyChanged();
            }
        }

        #endregion //data

        private bool _isUserEditable = true;
        public bool IsUserEditable
        {
            get { return _isUserEditable; }
            set
            {
                _isUserEditable = value;
                OnPropertyChanged();
            }
        }

        private double _currentProgress;
        public double CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand => _cancelCommand ??=
            new RelayCommand(async () => await OperationCancelled());

        private RelayCommand _proceedCommand;
        public RelayCommand ProceedCommand => _proceedCommand ??=
            new RelayCommand(() => OperationProceed());

        private async Task OperationCancelled()
        {
            var libraryUtil = App.Services.GetRequiredService<LibraryViewModel>();
            await libraryUtil.WallpaperDelete(libData);
        }

        private void OperationProceed()
        {
            JsonStorage<LivelyInfoModel>.StoreData(Path.Combine(libData.LivelyInfoFolderPath, "LivelyInfo.json"), libData.LivelyInfo);
            var libraryUtil = App.Services.GetRequiredService<LibraryViewModel>();
            //libraryUtil.SortWallpaper((LibraryModel)libData);
        }
    }
}
