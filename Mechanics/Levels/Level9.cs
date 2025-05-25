using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SomeTest.Maps;

public class Level9 : IScene
{
    private ContentManager contentManager;
    private SceneManager sceneManager;
    private GraphicsDevice graphicsDevice;
    private Player player;
    
    private EnemyManager enemyManager;
    public Boss _boss;
    
    private FireSpirit _fireSpirit2;
    private FireSpirit _fireSpirit3;
    private ArcaneArcher _arcaneArcher;
    private Death _death;
    private GoldSkeleton _goldSkeleton2;
    private ArcaneArcher _arcaneArcher2;
    
    private FireSpirit _fireSpirit;
    private Skeleton _skeleton;
    private GoldSkeleton _goldSkeleton;


    public bool IsBossDefeat; 
    public bool IsBossActive;
    private SpriteFont _font;
    private LoadMap mapFg;
    private LoadMap mapMg;
    private LoadMap mapCollision;
    private Texture2D texture;
    private Texture2D debugTexture;
    private bool _isWaveStarted = false;
    private bool stopWaveStage1 = false;
    private bool stopWaveStage2 = false;
    private float _fadeAlpha = 0f;

    private float timer = 0f;
    private bool _playerGetDagame = false;
    
    private Texture2D heatlhBar;
    private Song bossFight;
    public Level9(ContentManager contentManager, SceneManager sceneManager, GraphicsDevice graphicsDevice, Player player)
    {
        this.player = player;
        this.sceneManager = sceneManager;
        this.contentManager = contentManager;
        this.graphicsDevice = graphicsDevice;
    }

    public void Load()
    {
        heatlhBar = contentManager.Load<Texture2D>("BossHealthBar");
        texture = contentManager.Load<Texture2D>("TextureAtlas/All_content");
        bossFight = contentManager.Load<Song>("Audio/BossFight");
        
        mapFg = new LoadMap( "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapFg.LoadMapp("Level9/level9_fg.csv");
        mapMg = new LoadMap( "TextureAtlas/Dungeon", contentManager, graphicsDevice, 16);
        mapMg.LoadMapp("Level9/level9_mg.csv");
        mapCollision = new LoadMap( "TextureAtlas/ALL_content", contentManager, graphicsDevice, 16);
        mapCollision.LoadMapp("Level9/level9_collision.csv");
        
        debugTexture = new Texture2D(graphicsDevice, 1, 1);
        debugTexture.SetData(new[] { Color.White });
        _font = contentManager.Load<SpriteFont>("Fonts/Main");
        
        enemyManager = new EnemyManager();
        
        _boss = new Boss(contentManager, graphicsDevice, new Vector2(435, 445), player);
        _fireSpirit = new FireSpirit(contentManager, graphicsDevice, new Vector2(900, 50), player);
        _skeleton = new Skeleton(contentManager, graphicsDevice, new Vector2(350, 445), player);
        _goldSkeleton = new GoldSkeleton(contentManager, graphicsDevice, new Vector2(750, 445), player);

        _fireSpirit2 = new FireSpirit(contentManager, graphicsDevice, new Vector2(900, 50), player);
        _fireSpirit3 = new FireSpirit(contentManager, graphicsDevice, new Vector2(-100, 50), player);
        _arcaneArcher = new ArcaneArcher(contentManager, graphicsDevice, new Vector2(672, 250), player);
        _arcaneArcher2 = new ArcaneArcher(contentManager, graphicsDevice, new Vector2(400, 445), player);
        _goldSkeleton2 = new GoldSkeleton(contentManager, graphicsDevice, new Vector2(250, 445), player);
        _death = new Death(contentManager, graphicsDevice, new Vector2(240, 230), player);
        
        enemyManager.AddEnemy(_boss);
        MediaPlayer.Stop();
        
        player._position = new Vector2(215, 0);
        player._velocity.Y = 1;
        player.JumpForce = -400f;
    }

    public void Update(GameTime gameTime)
    {
        
        var a = player._hitboxRect.X;
        var b = player._hitboxRect.Width;
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_boss.health <= 400 && !stopWaveStage1)
        {
            
            Console.WriteLine("Start Immune stage");
            _boss.StartImmuneStage();
            if (!_isWaveStarted)
            {
                StartMonsterWave(enemyManager);
                _isWaveStarted = true;
            }

            if (_isWaveStarted)
            {
                mapCollision.Update(_skeleton);
                mapCollision.Update(_goldSkeleton);
            }
            
            if (_boss._isImmuneStage && enemyManager.GetEnemies().Count == 1)
            {
                Console.WriteLine("End Immune stage");
                _boss.StopImmuneStage();
                stopWaveStage1 = true;
                _isWaveStarted = false;
            }
        }
        if (_boss.health <= 200 && !stopWaveStage2)
        {
            Console.WriteLine("Start Immune stage");
            _boss.StartImmuneStage();
            if (!_isWaveStarted)
            {
                StartMonsterWave(enemyManager);
                _isWaveStarted = true;
            }

            if (_isWaveStarted)
            {
                mapCollision.Update(_death);
                mapCollision.Update(_goldSkeleton2);
                mapCollision.Update(_arcaneArcher);
                mapCollision.Update(_arcaneArcher2);
            }
            if (_boss._isImmuneStage && enemyManager.GetEnemies().Count == 1)
            {
                Console.WriteLine("End Immune stage");
                _boss.StopImmuneStage();
                stopWaveStage2 = true;
                _isWaveStarted = false;
            }
        }
        if (MediaPlayer.State != MediaState.Playing && _boss._isActivate)
        {
            MediaPlayer.Play(bossFight);
            MediaPlayer.Volume = 0.3f;
        }

        if (_boss.health <= 0)
        {
            timer += deltaTime;
            Console.WriteLine(timer);
            _fadeAlpha = Math.Min(_fadeAlpha + 0.2f * (float)gameTime.ElapsedGameTime.TotalSeconds, 1f);
        }
        // if (_boss.health < 450 && !_boss._isImmuneStage)
        // {
        //     Console.WriteLine("Start Immune stage");
        //     _boss.StartImmuneStage();
        //     StartMonsterWave(enemyManager);
        //     _isWaveStarted = true;
        //     
        //     // if (enemyManager.GetEnemies().Count == 1)
        //     // {
        //     //     Console.WriteLine("Stop Immune stage");
        //     //     _isWaveStarted = false;
        //     //     _boss.StopImmuneStage();
        //     // }
        // }
        //
        // if (enemyManager.GetEnemies().Count == 1 && _isWaveStarted)
        // {
        //     Console.WriteLine("End Immune stage");
        //     _boss.StopImmuneStage();
        // }
        //
            //Console.WriteLine(enemyManager.GetEnemies().Count);
        // if (enemyManager.GetEnemies().Count == 1)
        // {
        //     Console.WriteLine("Stop Immune stage");
        //     _isWaveStarted = false;
        //     _boss.StopImmuneStage();
        // }
        
        // else if (_isWaveStarted && enemyManager.GetEnemies().Count == 1 && !_boss._isImmuneStage)
        // {
        //     
        // }
        // Тут нужно будет спавнить мобов
        
        // if (player._hitboxRect.Y > 970)
        // {
        //     sceneManager.AddScene(new Level5(contentManager, sceneManager, graphicsDevice, player));
        // }
        mapCollision.Update(player);
        mapCollision.Update(_boss);
        enemyManager.Update(gameTime);
    }

    private void StartMonsterWave(EnemyManager enemyManager)
    {
        if (!_isWaveStarted)
        {
            if (_boss.health > 200 && _boss.health <= 400)
            {
                Console.WriteLine("Start 1");
                enemyManager.AddEnemy(_fireSpirit);
                enemyManager.AddEnemy(_skeleton);
                enemyManager.AddEnemy(_goldSkeleton);
            }

            if (_boss.health <= 200)
            {
                Console.WriteLine("Start 2");
                // Нужно создавать новый экземпляр класса _fireSpitir
                enemyManager.AddEnemy(_fireSpirit2);
                enemyManager.AddEnemy(_fireSpirit3);
                enemyManager.AddEnemy(_death);
                enemyManager.AddEnemy(_goldSkeleton2);
                enemyManager.AddEnemy(_arcaneArcher);
                enemyManager.AddEnemy(_arcaneArcher2);
            }
        }
        
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        mapMg.Draw(spriteBatch);
        mapFg.Draw(spriteBatch);
        enemyManager.Draw(spriteBatch);
        if (_boss._isActivate)
        {
            //spriteBatch.Draw(debugTexture, new Rectangle(271, 135, _boss.displayedBossHealt, 15), Color.Gray);
            spriteBatch.Draw(debugTexture, new Rectangle(271, 135, _boss.displayedBossHealt, 15), Color.Red);
            spriteBatch.Draw(heatlhBar, new Rectangle(250, 115, 520, 45), Color.White);
        }
        if (_boss.health <= 0 && timer <= 5.2f)
        {
            spriteBatch.Draw(debugTexture, 
                new Rectangle(0, 0, 960, 640), 
                Color.Black * _fadeAlpha);
            if (timer >= 5.0f && !_playerGetDagame)
            {
                player.TakeDamage(100);
                _playerGetDagame = true;
            }
        }
    }
    public int LevelNumber { get; } = 9;
}
