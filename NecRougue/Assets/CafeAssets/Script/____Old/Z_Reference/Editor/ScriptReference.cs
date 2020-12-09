using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// コードとしては意味がないが、備忘録として使う。
/// </summary>
public class Scriptリファレンス
{
    /*
     * todo
     * ・NpcSystem
     *
     * 方針
     * ・現状開発がとびとびになることがわかっているので、開発期間に間が空いても問題ない設計にしたい。
     * 
     * 
     * 設計
     *システムが巨大化することを考え、以下の点に配慮する
     * ・SOLID原則の遵守
     * ・クリーンアーキテクチャを適用
     * スクリプト配下を以下のように分ける。
     * ・Archtecture ... 全体設計に関するスクリプト
     * ・SceneInstaller ... SceneContextに配置している、MonoInstallerを継承したスクリプト シーンごとに分けている
     * ・Sequence ... シーケンス シーンごとに分けている
     * ・System ... 単一の仕様を責務を「システム」として切り分ける。システムには複数の責務が含まれていてもよい。
     * ・Debug
     * ・Test
     * 
     * ・シーンは一番大きい責務単位である。例外として、インストーラーは
     * ・
     * 
     * ・
     * 
     * システム一覧
     * 
     * 
     * 
     */
}
