﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EFT;
using UnityEngine;

// Token: 0x020007D4 RID: 2004
public class BaseSoundPlayer : MonoBehaviour
{
	// Token: 0x04002E93 RID: 11923
	[SerializeField]
	public List<BaseSoundPlayer.SoundElement> AdditionalSounds = new List<BaseSoundPlayer.SoundElement>();

	// Token: 0x04002E94 RID: 11924
	[SerializeField]
	public BaseSoundPlayer.SoundElement[] MainSounds;

	// Token: 0x020007D5 RID: 2005
	[Serializable]
	public class SoundElement
	{
		// Token: 0x04002E98 RID: 11928
		public string EventName = "";

		// Token: 0x04002E99 RID: 11929
		public int RollOff = 30;

		// Token: 0x04002E9A RID: 11930
		public float Volume = 1f;

		// Token: 0x04002E9B RID: 11931
		public AudioClip[] SoundClips;
	}
}
