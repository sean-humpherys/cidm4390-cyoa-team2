using CYOA;

var game = new Game();
game.StartGame();

if (game.Player != null)
{
    game.StartAdventure(() => game.StartAttack(new Combat()));
}