using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct MonsterSkill
{
        public AnimationClip _skill;
        public float _range;
        public float _coolTime;
}
public class EpicMonster : MonsterController
{
        [SerializeField] AnimationClip _normalAttack;
	[SerializeField] List<MonsterSkill> _skills = new List<MonsterSkill>();

        public override void Update()
        {
		//_anim.Play()

	}


}
