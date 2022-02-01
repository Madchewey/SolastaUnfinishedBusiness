using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Text;
using TA.AI;
using TA;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using  static  ActionDefinitions ;
using  static  TA . AI . DecisionPackageDefinition ;
using  static  TA . AI . DecisionDefinition ;
using  static  RuleDefinitions ;
using  static  BanterDefinitions ;
using  static  Gui ;
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(SubtitleTableDefinition))]
    public static partial class SubtitleTableDefinitionExtensions
    {
        public static System.Collections.Generic.List<SubtitleStyleDuplet> GetStyleDuplets<T>(this T entity)
            where T : SubtitleTableDefinition
        {
            return entity.GetField<System.Collections.Generic.List<SubtitleStyleDuplet>>("styleDuplets");
        }

        public static T SetLineHeight<T>(this T entity, System.Single value)
            where T : SubtitleTableDefinition
        {
            entity.SetField("lineHeight", value);
            return entity;
        }

        public static T SetLineSpacing<T>(this T entity, System.Single value)
            where T : SubtitleTableDefinition
        {
            entity.SetField("lineSpacing", value);
            return entity;
        }

        public static T SetWordSpacing<T>(this T entity, System.Single value)
            where T : SubtitleTableDefinition
        {
            entity.SetField("wordSpacing", value);
            return entity;
        }
    }
}