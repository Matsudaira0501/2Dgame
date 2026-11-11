# 2D横スクロールアクションゲーム（Unity / C#）

UnityとC#を使って個人制作した2Dアクションゲームのプロジェクトです。ゲームの企画、機能実装、動作確認、調整を行い、unityroomで公開しました。

## ブラウザでプレイ

**[▶ unityroomでゲームをプレイする](https://unityroom.com/games/kantansuginai)**

PCのWebブラウザからプレイできます。ソースコードを動かす場合は下記のUnityプロジェクトを開いてください。

## ゲーム概要

キャラクターを操作してステージ内のコインをすべて集めるとクリアになるアクションゲームです。移動・ジャンプの基本操作に加えて、難易度を変更するスイッチを用意しています。各条件を組み合わせて挑戦でき、すべての難易度スイッチを有効にした状態でのクリアタイムをランキングに送信する仕組みがあります。

## 操作方法

- 左右移動：← / → または A / D
- ジャンプ：↑ または Space
- タイトル画面から開始：Shift
- タイトル画面の効果音音量調整：← / → または A / D

キーボード操作を想定しています。

## 実装した主な機能

| 機能 | 内容 | 主な実装 |
| --- | --- | --- |
| キャラクター操作 | 左右移動、ジャンプ、接地判定、落下時のリスタート | [PlayerController.cs](Assets/Scripts/PlayerController.cs) |
| コイン収集とクリア判定 | コイン取得時のスコア更新、全コイン取得でクリア | [CoinPickup.cs](Assets/Scripts/CoinPickup.cs)、[GameClearManager.cs](Assets/Scripts/GameClearManager.cs) |
| 難易度の切り替え | 6種類の条件をスイッチで設定し、ステージの構成やプレイヤーの挙動へ反映 | [DifficultySettings.cs](Assets/Scripts/DifficultySettings.cs)、[StageDifficultyApplier.cs](Assets/Scripts/StageDifficultyApplier.cs) |
| 制限時間 | 選択した難易度に応じた時間制限と、時間切れ時のリスタート | [StageTimer.cs](Assets/Scripts/StageTimer.cs) |
| クリアタイム・ランキング | 経過時間の計測とunityroomランキングAPIへのスコア送信 | [ClearTimeTracker.cs](Assets/Scripts/ClearTimeTracker.cs)、[UnityroomRankingSubmitter.cs](Assets/Scripts/UnityroomRankingSubmitter.cs) |
| 画面・演出 | タイトル、チュートリアル、クリア表示、効果音設定など | [TitleManager.cs](Assets/Scripts/TitleManager.cs)、[TutorialManager.cs](Assets/Scripts/TutorialManager.cs) |

### 難易度の6項目

- コイン数の増加
- コイン配置の難化
- 足場の縮小
- トゲの増加
- 着地時の強制ジャンプ
- 制限時間の追加

## 処理の構成

ゲーム内の処理は、役割ごとに複数のスクリプトへ分けています。

```text
難易度選択
  DifficultySettings
    ├─ StageDifficultyApplier → コイン・足場・トゲ・ジャンプ条件
    └─ StageTimer             → 条件に応じた制限時間

プレイとクリア
  PlayerController → CoinPickup
                         ├─ ScoreManager       → スコア更新
                         └─ GameClearManager   → コイン取得数とクリア判定
                                  └─ ClearTimeTracker → UnityroomRankingSubmitter
```

※ランキングへの送信は、すべての難易度スイッチを有効にして最終クリアした場合を対象としています。

## 開発で工夫した点

**操作と当たり判定：** 接地判定に `Physics2D.OverlapBox` を用い、接地中のみジャンプできる処理を実装しました。また、スプライトに合わせて当たり判定の大きさを調整できるようにしています。

**難易度の構成：** 難易度スイッチの状態を `DifficultySettings` に集約し、`StageDifficultyApplier` でコイン位置・足場・トゲ・ジャンプ条件へ反映しています。難易度ごとにゲーム全体を別実装にせず、設定の組み合わせで挙動を変えられる構成にしました。

**記録とランキング：** クリアタイムは `Time.realtimeSinceStartup` に基づいて計測し、所定の条件でクリアした際にunityroomのランキング機能へ送信する処理を実装しています。

## 開発環境とプロジェクト構成

- 言語：C#
- ゲームエンジン：Unity **6000.4.7f1**（`ProjectSettings/ProjectVersion.txt` に記載）
- 主な機能：Unity 2D Physics、Input System、unityroom client library
- 公開先：unityroom（WebGL）

主要なディレクトリ：

- `Assets/Scripts/`：ゲームの処理
- `Assets/Scenes/`：タイトル、ステージ、難易度設定画面
- `Assets/images/`、`Assets/sounds/`、`Assets/tiles/`：ゲーム用アセット
- `Packages/`：パッケージ設定
- `ProjectSettings/`：Unityプロジェクト設定

ローカルで確認する場合は、Unity Hubからこのリポジトリをプロジェクトとして開いてください。編集・実行にはプロジェクトに記載されたUnityバージョンを推奨します。

## 補足

このリポジトリは個人制作・学習の過程を含むプロジェクトです。画像・音声などのアセットには第三者が権利を持つ素材が含まれる可能性があるため、コード以外の素材を再利用する際は個別に利用条件を確認してください。
