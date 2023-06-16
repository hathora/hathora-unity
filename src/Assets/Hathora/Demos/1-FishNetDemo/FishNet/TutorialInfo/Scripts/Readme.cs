using System;
using UnityEngine;

// (!) Added ns to prevent conflicts in 2D Space Shooter, and probably others
namespace Hathora.Demos._1_FishNetDemo.FishNet.TutorialInfo.Scripts
{
	public class Readme : ScriptableObject {
		public Texture2D icon;
		public string title;
		public Section[] sections;
		public bool loadedLayout;
	
		[Serializable]
		public class Section {
			public string heading, text, linkText, url;
		}
	}
}
