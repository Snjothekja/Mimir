import { useState } from 'react'
import './home.css'
import * as React from 'react';
function home() {

    interface PostInfo {
        upfp: TexImageSource
        posteruid: number
        postid: number
        posttext?: string
        postimage?: TexImageSource
        likeamt?: number
        commentamt?: number
        repostamt?: number
    }

    interface PostArrayInterface {
        posteruid: number
        postid: number
    }

    interface UserAccountInterface {
        tokenValidBool: boolean
        username: string
        pfp: string
        profiledesc: string
        followersamt: number
        followingamt: number
    }

    const CheckToken = async () => {
        let data = "";
        try {      
            const accountToken = localStorage.getItem("sessionid");
            const accountUID = localStorage.getItem("uid");
            if (accountToken === null) {
                window.location.assign("index.html");
            }
            const uidToken = accountUID + ":" + accountToken;
            const response = await fetch('api/mimirchecktoken?uidAndToken=' + uidToken.toString(), {
                method: "POST"
            })
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`)
            }
            data = response.json().toString();

        }
        catch (err) {
            console.error('Error loggin in:', err)
        }  
        if (data === "false") {
            alert(data);
            window.location.assign("index.html");
        }
    }
    function GetNextPost () {

    }

    function ClickedHome() {

    }

    function ClickedFeeds() {

    }

    const ClickedAccount = async () => {
        const accountToken = localStorage.getItem("sessionid");
        const accountUID = localStorage.getItem("uid");
        const uidToken = accountUID + ":" + accountToken;
        alert("Clicked Account: " + "../api/mimirpostgresgetuseraccount?uidAndToken=" + uidToken.toString());
        try {
            const response = await fetch('../api/mimirpostgresgetuseraccount?uidAndToken=' + uidToken.toString(), {
                method: "POST"
            })
            if (!response.ok) {
                alert("Clicked Account Response Fail");
                throw new Error(`HTTP error! status: ${response.status}`)
            }
            const data: UserAccountInterface[] = await response.json()
            
            /* Promised array { tokenValidBool, username, pfp, profiledesc, followersamt, followingamt } */
            if (data[0].toString() === "false") {
                window.location.assign("index.html");
            }
            alert(data[0] + " " + data[1] + " " + data[2]);
            const accountpfp = document.getElementById("accountpfp") as HTMLImageElement;

            if (data[2].toString() === "") {
                accountpfp.src = "https://upload.wikimedia.org/wikipedia/commons/a/ac/Default_pfp.jpg";
            }
            else {
                accountpfp.src = data[2].toString();
            }

            document.getElementById("accountname").textContent = data[1].toString();

            if (data[3].toString() != "") {
                document.getElementById("accountdesc").textContent = data[3].toString();
            }
            
        }
        catch (err) {
            console.error('Error getting account information in:', err)
        }
        
    }

    function ClickedNewPost() {

    }

    function ClickedForeignAccount() {

    }

    function handleAccountButton(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault()
        ClickedAccount();
    }

  return (
      <main className="main" onLoad={CheckToken}>
          <div id="sidebar" className="sidenav">
              <button>Home</button>
              <button>Feeds</button>
              <div id="accountSideNav" className="sidenavitem">
                  <button onClick={handleAccountButton}>Account</button>
              </div>   
              <button>New Post</button>
          </div>
          <div id="body">
              <div id="home">
              </div>
              <div id="feeds">
              </div>
              <div id="account">
                  <img src="https://upload.wikimedia.org/wikipedia/commons/a/ac/Default_pfp.jpg" width="128" height="128" className="profileimage" id="accountpfp" />
                  <div id="accountname" className="profilename">
                  Username
                  </div>
                  <div id="accountdesc" className="profiledesc">
                      <input
                          type="text"
                          id="uname"
                          name="name"
                          className="profiledesc"
                          placeholder="Add a description!" />
                  </div>
                  <div id="followeramt">
                  0
                  </div>
                  <div id="followingamt">
                  0
                  </div>
              </div>
              <div id="newpost">
              </div>
              <div id="foreignaccount">
              </div>
          </div>
      </main>
    );
}

export default home;