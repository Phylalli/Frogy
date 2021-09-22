using System;
using System.Collections.Generic;

namespace FrogyCore.Models
{
    [Serializable]
    public enum SoftwareType
    {
        Win32 = 0,
        UWP = 1,
        PWA = 2,
        Android = 3,
        iOS = 4,
        Linux = 5,
        Unspecified = 6
    }

    /// <summary>
    /// Software class.
    /// </summary>
    [Serializable]
    public class Software
    {
        public string ID { get; set; }

        /// <summary>
        /// string - GUID of other device
        /// string - Software ID on other device
        /// </summary>
        public Dictionary<string, string> AliasID { get; set; } = new Dictionary<string, string>();

        public string NickName { get; set; }

        public string Name { get; set; }

        public string IconMD5 { get; set; }

        public string Path { get; set; }

        public List<string> Categories { get; set; } = new List<string>();

        public TimeSpan DailyQuota { get; set; } = new TimeSpan(0, 0, 0);

        public SoftwareType SoftwareType { get; set; }

        public List<SubSoftware> LinkedSoftwares { get; set; } = new List<SubSoftware>();

        public Software(string id, string name, string iconMD5, string path, SoftwareType softwareType)
        {
            ID = id;
            Name = name;
            IconMD5 = iconMD5;
            Path = path;
            SoftwareType = softwareType;
        }

        /// <summary>
        /// Make a software subsoftware of this. (Without deleting the old one)
        /// </summary>
        /// <param name="software">Software to be linked.</param>
        public void LinkSoftware(Software software)
        {
            LinkedSoftwares.Add(new SubSoftware(software.ID, software.Name, software.Path));
        }

        /// <summary>
        /// Make a remote software id an alias of a local software id.
        /// </summary>
        /// <param name="guid">Guid of remote device.</param>
        /// <param name="id">Software id on remote device.</param>
        public void LinkAliasID(string guid, string id)
        {
            AliasID.Add(guid, id);
        }
    }

    [Serializable]
    public class SubSoftware
    {
        public string OriginalID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public SubSoftware(string id, string name, string path)
        {
            OriginalID = id;
            Name = name;
            Path = path;
        }
    }

    [Serializable]
    public class SoftwareInfo
    {
        public List<Software> SoftwareList = new List<Software>();

        /// <summary>
        /// Get software ID by name and path. Generate new if not exist.
        /// </summary>
        /// <param name="name">Software name</param>
        /// <param name="path">Software path</param>
        /// <param name="iconMD5">MD5 of software icon</param>
        /// <returns>Software ID</returns>
        public string MatchSoftwareID(string name, string path, string iconMD5, SoftwareType softwareType)
        {
            if (SoftwareList.Count > 0)
            {
                foreach (Software software in SoftwareList)
                {
                    if (software.Name == name && software.Path == path)
                    {
                        software.IconMD5 = iconMD5;
                        return software.ID;
                    }
                    if (software.LinkedSoftwares.Count > 0)
                    {
                        foreach (SubSoftware subSoftware in software.LinkedSoftwares)
                        {
                            if (subSoftware.Name == name && subSoftware.Path == path)
                            {
                                software.IconMD5 = iconMD5;
                                return software.ID;
                            }
                        }
                    }
                }
            }
            string result = Guid.NewGuid().ToString("N");
            SoftwareList.Add(new Software(result, name, iconMD5, path, softwareType));
            return result;
        }

        /// <summary>
        /// Get software by software ID.
        /// </summary>
        /// <param name="id">Software ID</param>
        /// <returns>Software. Null if no id matched.</returns>
        public Software MatchSoftware(string id)
        {
            if (SoftwareList.Count > 0)
            {
                foreach (Software software in SoftwareList)
                {
                    if (software.ID == id)
                    {
                        return software;
                    }

                    if (software.LinkedSoftwares.Count > 0)
                    {
                        foreach (SubSoftware subSoftware in software.LinkedSoftwares)
                        {
                            if (subSoftware.OriginalID == id)
                            {
                                return software;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Make a software subsoftware of this.
        /// </summary>
        /// <param name="softwareID">Software id be linked to.</param>
        /// <param name="substringID">Software id to be linked.</param>
        public void LinkSoftware(string softwareID, string subsoftwareID)
        {
            if (SoftwareList.Count > 0)
            {
                foreach (Software software in SoftwareList)
                {
                    if (software.ID == softwareID)
                    {
                        foreach (Software subSoftware in SoftwareList)
                        {
                            if (subSoftware.ID == subsoftwareID)
                            {
                                software.LinkSoftware(subSoftware);
                                SoftwareList.Remove(subSoftware);
                                return;
                            }
                        }

                    }
                }
            }
            return;
        }

        /// <summary>
        /// Make a remote software id an alias of a local software id.
        /// </summary>
        /// <param name="id">Software id on local device.</param>
        /// <param name="guid">Guid of remote device.</param>
        /// <param name="aliasID">Software id on remote device.</param>
        /// <returns></returns>
        public void LinkAliasID(string id, string guid, string aliasID)
        {
            if (SoftwareList.Count > 0)
            {
                foreach (Software software in SoftwareList)
                {
                    if (software.ID == id)
                    {
                        software.LinkAliasID(guid, aliasID);
                        return;
                    }
                }
            }
            return;
        }
    }
}
