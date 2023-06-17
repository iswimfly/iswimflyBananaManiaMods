using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Flash2;
using Framework.UI;
using UnityEngine;
using static Flash2.Chara;

namespace Guest
{
    class MonkeeAudio
    {
        public void playSfx(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx, float timer, bool isGoal)
        {
            audio.SetCue(acb, sfx);

            if (audio.GetStatus() == CriAtomExPlayer.Status.Playing && timer >= 0.2f && !isGoal)
            {
                audio.Stop();
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(1.0f);
                audio.Update(playback);
            }
            else if (audio.GetStatus() == CriAtomExPlayer.Status.Playing && timer <= 0.2f || isGoal)
            {
                // do nothing
            }
            else
            {
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(1.0f);
                audio.Update(playback);
            }
        }

        public void playOtto(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);
            audio.Start();
        }

        public void playFallout(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);
            if (audio.GetStatus() != CriAtomExPlayer.Status.Playing)
            {
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(1.0f);
                audio.Update(playback);
            }
        }

        public void playGoal(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);
            audio.Start();
        }

        public void playGoalFly(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);
            audio.Start();
        }

        public void playStartSfx(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);
            if (audio.GetStatus() != CriAtomExPlayer.Status.Playing)
            {
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(1.0f);
                audio.Update(playback);
            }
        }
    }

    class Monkee : MonkeeAudio
    {
        public void calcImpact(CriAtomExPlayer audio, CriAtomExAcb acb, float impact, Player player, float timer, bool isGoal)
        {
            if (impact <= 6.5)
            {
                playSfx(audio, acb, "goal_fly", timer, isGoal); ;
            }
            else if (impact > 6.5 && impact <= 9.5)
            {

                playSfx(audio, acb, "hanauta", timer, isGoal);
            }
            else
            {
                if (player.charaKind.ToString().ToLower() == "yanyan")
                {
                    playSfx(audio, acb, "yabai_long", timer, isGoal);
                }
                else
                {
                    playSfx(audio, acb, "thankyou", timer, isGoal);
                }
            }
        }

        public void calcBanana(CriAtomExPlayer audio, CriAtomExAcb acb, int banana, float timer, bool isGoal)
        {
            if (banana == 1)
            {
                playSfx(audio, acb, "fallout", timer, isGoal);
            }
            else if (banana == 10)
            {
                playSfx(audio, acb, "timeover", timer, isGoal);
            }
        }

        public void start(CriAtomExPlayer audio, CriAtomExAcb acb)
        {
            playStartSfx(audio, acb, "start");
        }

        public void unbalance(CriAtomExPlayer audio, CriAtomExAcb acb)
        {
            playOtto(audio, acb, "happy_long");
        }

        public void scream(CriAtomExPlayer audio, CriAtomExAcb acb)
        {
            playFallout(audio, acb, "happy");
        }

        public void goal(CriAtomExPlayer audio, CriAtomExAcb acb)
        {
            playGoal(audio, acb, "continue_unselect");
        }

        public void goalFly(CriAtomExPlayer audio, CriAtomExAcb acb)
        {
            playGoalFly(audio, acb, "yabai_long");
        }
    }

    class BananaAudio
    {
        public void playSfx(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {

            audio.SetCue(acb, sfx);

            if (sfx == "timer")
            {
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(0.9f);
                audio.Update(playback);
            }
            else
            {
                audio.Start();
            }
        }

        public void playSfxLoud(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {

            audio.SetCue(acb, sfx);

            if (sfx == "timer")
            {
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(1.0f);
                audio.Update(playback);
            }
            else
            {
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(1.2f);
                audio.Update(playback);
            }
        }
    }

    class Banana : BananaAudio
    {
        public void calcCollected(CriAtomExPlayer audio, CriAtomExAcb acb, int banana)
        {
            if (banana == 1)
            {
                playSfx(audio, acb, "bananaget");
            }
            else if (banana == 10)
            {
                playSfx(audio, acb, "timer");
            }
        }

        public void calcGuestCollected(CriAtomExPlayer audio, CriAtomExAcb acb, int banana)
        {
            if (banana == 1)
            {
                playSfxLoud(audio, acb, "bananaget");
            }
            else if (banana == 10)
            {
                playSfxLoud(audio, acb, "timer");
            }
        }
    }

    internal class GuestCharacters : MonoBehaviour
    {
        public GuestCharacters(IntPtr value) : base(value) { }

        private Player _player;
        private Monkee _monkee;
        private Banana _banana;

        private CriAtomExPlayer _monkeePlayer;
        private CriAtomExPlayer _bananaPlayer;

        private CriAtomExAcb _monkeeAcb;
        private CriAtomExAcb _bananaAcb;

        // These are ugly, I don't want to keep them
        private List<float> _boundArray;
        private List<float> _collideArray;
        private List<float> _dropArray;
        private List<float> _softArray;

        private string[] _monkeeArray;
        private string[] _guestArray;
        private string[] _consoleArray;
        private string[] _dlcArray;
        private string _monkeeType;
        private string monkeePath;
        private string bananaPath;

        private int _harvestedBananas;
        private int _timer;

        private float _bufferTime;
        private float _intensity;

        private bool _isStart;
        private bool _isOffBalance;
        private bool _isFallout;
        private bool _isGoal;
        private bool _isGoalFly;
        private bool _isBumped;

        private void Awake()
        {
            _player = FindObjectOfType<MainGameStage>().GetPlayer();

            _monkee = new Monkee();
            _banana = new Banana();

            _boundArray = new List<float>();
            _collideArray = new List<float>();
            _dropArray = new List<float>();
            _softArray = new List<float>();

            _monkeeArray = new string[] { "aiai", "baby", "doctor", "gongon", "jam", "jet", "meemee", "yanyan" };
            _guestArray = new string[] { "beat", "kiryu", "sonic", "tails", "dlc01", "dlc02", "dlc03" };
            _consoleArray = new string[] { "dreamcast", "gamegear", "segasaturn" };
            

            _monkeeType = _player.charaKind.ToString().ToLower();


            if (_consoleArray.Contains(_monkeeType))
            {
                monkeePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Consoles\vo_" + _monkeeType + ".acb");
                _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, monkeePath, null);

                bananaPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Bananas\bananas.acb");
                _bananaAcb = CriAtomExAcb.LoadAcbFile(null, bananaPath, null);
            }
            else if (_guestArray.Contains(_monkeeType))
            {
                monkeePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Guests\vo_" + _monkeeType + ".acb");
                _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, monkeePath, null);

                bananaPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Bananas\bananas_" + _monkeeType + ".acb");
                _bananaAcb = CriAtomExAcb.LoadAcbFile(null, bananaPath, null);
            }
            else
            {
                monkeePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\DLC\vo_muted.acb");
                _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, monkeePath, null);

                bananaPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Bananas\bananas_muted.acb");
                _bananaAcb = CriAtomExAcb.LoadAcbFile(null, bananaPath, null);
            }

            // Create players for each sfx
            _bananaPlayer = new CriAtomExPlayer();
            _monkeePlayer = new CriAtomExPlayer();

            _timer = MainGame.mainGameStage.m_GameTimer;

            _isStart = false;
            _isFallout = false;
            _isGoal = false;
            _isGoalFly = false;
            _isBumped = false;
        }

        private void Update()
        {
            int mainBananas = MainGame.mainGameStage.m_HarvestedBananaCount;
            var playerState = _player.m_PlayerMotion.m_State;
            var playerBalance = _player.m_PlayerMotion.m_UnbalanceState;
            var bumperAmount = _player.m_MainGameStage.m_Bumpers._size;
            var bumper = _player.m_MainGameStage.getNearestBumper(_player.m_PhysicsBall.m_Pos);

            _bufferTime += Time.deltaTime;

            if (_consoleArray.Contains(_monkeeType) || _guestArray.Contains(_monkeeType))
            {

                if (_player.m_MainGameStage.m_ReadyGoSequence.isFinished && !_isStart)
                {
                    _monkee.start(_monkeePlayer, _monkeeAcb);
                    _isStart = true;
                }

                if (playerBalance != PlayerMotion.UnbalanceState.NONE && !_isOffBalance)
                {
                    _monkee.unbalance(_monkeePlayer, _monkeeAcb);
                    _isOffBalance = true;
                }
                else if (playerBalance == PlayerMotion.UnbalanceState.NONE && _isOffBalance)
                {
                    _isOffBalance = false;
                }

                if (_player.IsFallOut() && !_isFallout)
                {
                    _monkee.scream(_monkeePlayer, _monkeeAcb);
                    _isFallout = true;
                }

                if (playerState == PlayerMotion.State.GOAL && !_isGoal)
                {
                    _monkee.goal(_monkeePlayer, _monkeeAcb);
                    _isGoal = true;
                }

                if (playerState == PlayerMotion.State.GOAL_FLY && !_isGoalFly)
                {
                    _monkee.goalFly(_monkeePlayer, _monkeeAcb);
                    _isGoalFly = true;
                }


                if (_guestArray.Contains(_monkeeType) && _harvestedBananas != mainBananas)
                {
                    int collected = mainBananas - _harvestedBananas;
                    _harvestedBananas = mainBananas;
                    _banana.calcGuestCollected(_bananaPlayer, _bananaAcb, collected);
                    _monkee.calcBanana(_monkeePlayer, _monkeeAcb, collected, _bufferTime, _isGoal);
                    _bufferTime = 0;
                }
                else if (_harvestedBananas != mainBananas)
                {
                    int collected = mainBananas - _harvestedBananas;
                    _harvestedBananas = mainBananas;
                    _banana.calcCollected(_bananaPlayer, _bananaAcb, collected);
                    _monkee.calcBanana(_monkeePlayer, _monkeeAcb, collected, _bufferTime, _isGoal);
                    _bufferTime = 0;
                }

                if (_player.m_BoundTimer > 0 && bumperAmount != 0)
                {
                    _collideArray.Clear();
                    _dropArray.Clear();
                    _softArray.Clear();

                    if (bumper.m_state == Bumper.State.HIT)
                    {
                        _isBumped = true;
                        Vector3 normal = _player.m_PhysicsBall.m_Pos.normalized;

                        int i = 0;
                        float vectorDot = Vector3.Dot(normal, _player.m_relativeVelo);
                        _intensity = Math.Abs(vectorDot);
                        _boundArray.Insert(i++, _intensity);
                    }
                    else
                    {
                        Vector3 normal = _player.m_PhysicsBall.m_Pos.normalized;

                        int i = 0;
                        float vectorDot = Vector3.Dot(normal, _player.m_relativeVelo);
                        _intensity = Math.Abs(vectorDot);
                        _boundArray.Insert(i++, _intensity);
                    }

                }
                else if (_player.m_BoundTimer > 0 && bumperAmount == 0)
                {
                    _collideArray.Clear();
                    _dropArray.Clear();
                    _softArray.Clear();

                    Vector3 normal = _player.m_PhysicsBall.m_Pos.normalized;

                    int i = 0;
                    float vectorDot = Vector3.Dot(normal, _player.m_relativeVelo);
                    _intensity = Math.Abs(vectorDot);
                    _boundArray.Insert(i++, _intensity);

                }

                if (_player.m_BoundTimer <= 0 && _boundArray.Any() && !_isBumped)
                {
                    int timePast = _timer - MainGame.mainGameStage.m_GameTimer;
                    if (timePast <= 10)
                    {
                        _boundArray.Clear();
                        _bufferTime = 0;
                    }
                    else
                    {
                        float maxIntensity = _boundArray.Max();
                        _monkee.calcImpact(_monkeePlayer, _monkeeAcb, maxIntensity, _player, _bufferTime, _isGoal);
                        _boundArray.Clear();
                        _bufferTime = 0;
                    }
                }
                else if (_player.m_BoundTimer <= 0 && _boundArray.Any() && _isBumped)
                {
                    float maxIntensity = _boundArray.Max();
                    _monkee.calcImpact(_monkeePlayer, _monkeeAcb, maxIntensity, _player, _bufferTime, _isGoal);
                    _boundArray.Clear();
                    _bufferTime = 0;
                    _isBumped = false;
                }
                
            }   
        }

        private void OnDisable()
        {
            _monkeePlayer.Stop();
            _monkeePlayer.Dispose();

            _bananaPlayer.Stop();
            _bananaPlayer.Dispose();
        }
    }
}

