using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using ChessDotNet;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChessApi.Controllers
{
    [Route("api/chess")]
    [ApiController]
    public class ChessController : Controller
    {
        ChessGame game;

        //start game
        [HttpGet]
        [Route("/startgame")]
        public int StartGame(){
            game = ChessGame.GetGame();
            return 1;
        }

        //parameters: 1 --> which player is moving
        //            2 --> what original pos
        //            3 --> new pos
        //            data: "play-original x-original y-new x-new y"
        [HttpPost]
        [Route("/move")]
        public bool Move()
        {
            var requestbody = "";
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                requestbody = reader.ReadToEnd();
            }
            var datas = requestbody.Split("-");
            //datas[0] --> player      datas[1] --> original    datas[2] --> new
            Move move = new Move(datas[1], datas[2], MakePlayer(datas[0]));    
            if(datas[0] != game.WhoseTurn.ToString()){
                return false;
            }
            if(!game.IsValidMove(move)){
                return false;
            }
            game.ApplyMove(move, true);
            // need to add special conditions here!
            // not finished yet!
            return true;
        }

        //check win or not
        [HttpGet]
        [Route("/checkwin")]
        public string CheckWin(){
            if (game.IsStalemated(Player.White) || game.IsStalemated(Player.Black))
                return "tie";
            if (game.IsWinner(Player.White))
                return "white";
            if (game.IsWinner(Player.Black))
                return "black";
            return "continue";
        }

        // extra method
        public Player MakePlayer(string p){
            if (p == "White")
                return Player.White;
            if (p == "Black")
                return Player.Black;
            else return Player.None;
        }
    }
}
