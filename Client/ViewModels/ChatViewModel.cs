﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ChatViewModel : ViewModelBase
    {
        private string name;
        private string photoPath;
        private string uniqueName;
        private bool isPrivate;
        private int maxUsers;
        private bool isPM;

        public int Id { get; set; }
        
        public string Name { get => name; set => SetProperty(ref name, value); }
        public string PhotoPath { get => photoPath; set => SetProperty(ref photoPath, value); }
        public string UniqueName { get => uniqueName; set => SetProperty(ref uniqueName, value); }
        public bool IsPrivate { get => isPrivate; set => SetProperty(ref isPrivate, value); }
        public int MaxUsers { get => maxUsers; set => SetProperty(ref maxUsers, value); }
        public bool IsPM { get => isPM; set => SetProperty(ref isPM, value); }
    }
}