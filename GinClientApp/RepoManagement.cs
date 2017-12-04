﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GinClientApp.GinClientService;
using GinClientLibrary;
using Newtonsoft.Json;

namespace GinClientApp
{
    public partial class RepoManagement : Form
    {
        private readonly GinClientServiceClient _client;
        private GinRepository[] _repositories;
        private bool _suppressEvents;
        private GinRepository _selectedRepository;

        public RepoManagement(GinClientServiceClient client)
        {
            InitializeComponent();
            _client = client;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void RepoManagement_Load(object sender, EventArgs e)
        {
            _repositories = _client.GetRepositoryList();

            foreach (var repo in _repositories)
            {
                lvwRepositories.Items.Add(repo.Name);
            }
        }

        private void lvwRepositories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwRepositories.SelectedIndices.Count <= 0)
            {
                _selectedRepository = null;
                DisableControls();
                return;
            }

            _suppressEvents = true;
            var repoName = lvwRepositories.SelectedItems[0];
            _selectedRepository = _repositories.Single(r => string.Compare(r.Name, repoName.Text) == 0);
            txtRepoName.Text = _selectedRepository.Name;
            txtGinCommandline.Text = _selectedRepository.Commandline;
            txtServerAddress.Text = _selectedRepository.ServerAddress;
            txtMountpoint.Text = _selectedRepository.Mountpoint.FullName;
            _suppressEvents = false;
            EnableControls();
        }

        private void EnableControls()
        {
            foreach (var ctrl in Controls)
            {
                if (ctrl == txtMountpoint || ctrl == txtPhysdir) continue;

                ((Control) ctrl).Enabled = true;
            }
        }

        private void DisableControls()
        {
            foreach (var ctrl in Controls )
            {
                if (ctrl == lvwRepositories) continue;
                
                ((Control) ctrl).Enabled = false;
            }
        }

        private void RepoManagement_FormClosing(object sender, FormClosingEventArgs e)
        {
            _client.UnmmountAllRepositories();

            foreach (var repo in _repositories)
            {
                _client.AddRepository(repo.PhysicalDirectory.FullName, repo.Mountpoint.FullName, repo.Name, repo.Commandline);
            }
            
            string saveFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                              @"\gnode\GinWindowsClient\SavedRepositories.json";
            if (File.Exists(saveFile))
                File.Delete(saveFile);

            var fs = File.CreateText(saveFile);
            fs.Write(JsonConvert.SerializeObject(_repositories));
            fs.Flush();
            fs.Close();
        }

        private void txtRepoName_TextChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;
            if (_selectedRepository == null) return;

            _selectedRepository.Name = txtRepoName.Text;
        }

        private void txtServerAddress_TextChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;
            if (_selectedRepository == null) return;

            _selectedRepository.ServerAddress = txtServerAddress.Text;
        }

        private void txtGinCommandline_TextChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;
            if (_selectedRepository == null) return;

            _selectedRepository.Commandline = txtGinCommandline.Text;
        }
    }
}