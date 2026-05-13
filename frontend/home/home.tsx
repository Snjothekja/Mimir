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
        profilebanner: string
    }

    let setFilePFP: File;
    let setFileBanner: File;
    let lastDate: Date;
    let postImage: File;

    const onFileChangePFP = (event) => {
        setFilePFP = event.target.files[0];
    }

    const onFileChangeBanner = (event) => {
        setFileBanner = event.target.files[0];
    }

    const onFileChangePostImage = (event) => {
        postImage = event.target.files[0];
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
    function GetNextPost() {

    }

    function ClickedHome() {

    }

    function ClickedFeeds() {

    }

    const ClickedAccount = async () => {
        const accountToken = localStorage.getItem("sessionid");
        const accountUID = localStorage.getItem("uid");
        const uidToken = accountUID + ":" + accountToken;
        /* alert("Clicked Account: " + "../api/mimirpostgresgetuseraccount?uidAndToken=" + uidToken.toString()); */
        try {
            const response = await fetch('../api/mimirpostgresgetuseraccount?uidAndToken=' + uidToken.toString(), {
                method: "POST"
            })
            if (!response.ok) {
                alert("Clicked Account Response Fail");
                throw new Error(`HTTP error! status: ${response.status}`)
            }
            const data: UserAccountInterface[] = await response.json()

            /* Promised array { tokenValidBool, username, pfp, profiledesc, followersamt, followingamt, banner } */
            if (data[0].toString() === "false") {
                window.location.assign("index.html");
            }
            /*alert(data[0] + " " + data[1] + " " + data[2] + " " + data[3] + " " + data[4] + " " + data[5] + " " + data[6]);*/
            const accountpfp = document.getElementById("accountpfp") as HTMLImageElement;
            const accountbanner = document.getElementById("accountbanner") as HTMLImageElement;

            document.getElementById("accountname").textContent = data[1].toString();

            if (data[3].toString() != "") {
                document.getElementById("editaccountdesc").textContent = data[3].toString();
            }

            if (data[2].toString() === "") {
                accountpfp.src = "https://upload.wikimedia.org/wikipedia/commons/a/ac/Default_pfp.jpg";
            }
            else {
                accountpfp.src = data[2].toString();
            }

            if (data[6].toString() === "") {
                accountbanner.src = "/tmpbanner";
            }
            else {
                accountbanner.src = data[6].toString();
            }

            

        }
        catch (err) {
            console.error('Error getting account information in:', err)
        }

    }

    const SubmitNewPost = async(postText: string) => {
        const accountToken = localStorage.getItem("sessionid");
        const accountUID = localStorage.getItem("uid");

        const content = new FormData();
        content.append("token", localStorage.getItem("sessionid"));
        content.append("uid", localStorage.getItem("uid"));
        content.append("postText", postText);
        content.append("image", setFileBanner);

        try {
            const response = await fetch('../api/mimirnewpost', {
                method: "POST",
                body: content
            })
            if (!response.ok) {
                alert("Clicked Account Response Fail");
                throw new Error(`HTTP error! status: ${response.status}`)
            }
        }
        catch (err) {
            console.error('Error setting description: ', err)
        }
        ClickedAccount();
    }

    function ClickedForeignAccount() {

    }

    const EditDescription = async (description: string) => {
        const accountToken = localStorage.getItem("sessionid");
        const accountUID = localStorage.getItem("uid");
        const UIDTokenChangeText = accountUID + ":" + accountToken + ":desc:" + description;
        
        try {
            const response = await fetch('../api/mimirpostgresupdatedesc?UIDTokenChangeText=' + UIDTokenChangeText.toString(), {
                method: "POST"
            })
            if (!response.ok) {
                alert("Clicked Account Response Fail");
                throw new Error(`HTTP error! status: ${response.status}`)
            }
        }
        catch (err) {
            console.error('Error setting description: ', err)
        }
        ClickedAccount();
    }

    const UpdateProfilePicture = async () => {
        const content = new FormData();
        content.append("token", localStorage.getItem("sessionid"));
        content.append("uid", localStorage.getItem("uid"));
        content.append("image", setFilePFP);

        try {
            const response = await fetch('../api/mimirupdatepfp', {
                method: "POST",
                body: content
            })
            if (!response.ok) {
                alert("Clicked Account Response Fail");
                throw new Error(`HTTP error! status: ${response.status}`)
            }
        }
        catch (err) {
            console.error('Error setting description: ', err)
        }
        ClickedAccount();
    }

    const UpdateProfileBanner = async () => {
        const content = new FormData();
        content.append("token", localStorage.getItem("sessionid"));
        content.append("uid", localStorage.getItem("uid"));
        content.append("image", setFileBanner);

        try {
            const response = await fetch('../api/mimirupdatebanner', {
                method: "POST",
                body: content
            })
            if (!response.ok) {
                alert("Clicked Account Response Fail");
                throw new Error(`HTTP error! status: ${response.status}`)
            }
        }
        catch (err) {
            console.error('Error setting description: ', err)
        }
        ClickedAccount();

    }

    function handleAccountButton(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault()
        ClickedAccount();
    }

    function handleEditDescriptionButton(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();
        const editDescField = document.getElementById("editprofiledesc");
        editDescField.style.display = "flex"
    }

    function handleEditPFPButton(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();
        const editDescField = document.getElementById("editpfp");
        editDescField.style.display = "flex"
    }

    function handleEditBannerButton(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();
        const editDescField = document.getElementById("editbanner");
        editDescField.style.display = "flex"
    }

    function handleDescInputSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();
        const editprofiledesc = document.getElementById("editprofiledesc");
        editprofiledesc.style.display = "none";
        if (event.currentTarget.desctextarea.value === undefined) { return; }
        EditDescription(event.currentTarget.desctextarea.value);
    }

    function handlePFPImageFileUpload(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();
        UpdateProfilePicture();
        const editpfp = document.getElementById("editpfp");
        editpfp.style.display = "none";
    }

    function handleProfileBannerImageUpload(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();
        UpdateProfileBanner();
        const editbanner = document.getElementById("editbanner");
        editbanner.style.display = "none";
    }

    function handleNewPostButton(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();
        const newPostField = document.getElementById("newposteditor");
        newPostField.style.display = "flex"
        
    }

    function handleNewPostSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();
        const newposteditor = document.getElementById("newposteditor");
        newposteditor.style.display = "none";
        if (event.currentTarget.newposttextarea.value === undefined) { return; }
        SubmitNewPost(event.currentTarget.newposttextarea.value);
    }

    window.onclick = function (event) {
        const editprofiledesc = document.getElementById("editprofiledesc");
        if (event.target == editprofiledesc) {
            editprofiledesc.style.display = "none";
        }
        const editpfp = document.getElementById("editpfp");
        if (event.target == editpfp) {
            editpfp.style.display = "none";
        }
        const editbanner = document.getElementById("editbanner");
        if (event.target == editbanner) {
            editbanner.style.display = "none";
        }
        const newpost = document.getElementById("newposteditor");
        if (event.target == newpost) {
            newpost.style.display = "none";
        }
    }

    function appendPosts() {
        
    }

  return (
      <main className="main" onLoad={CheckToken}>
          <div id="sidebar" className="sidenav">
              <div id="accountSideNav" className="sidenavitem">
                  <button>Home</button>
              </div>

              <div id="accountSideNav" className="sidenavitem">
                  <button>Feeds</button>
              </div>

              <div id="accountSideNav" className="sidenavitem">
                  <button onClick={handleAccountButton}>Account</button>
              </div>

              <div id="accountSideNav" className="sidenavitem">
                  <button onClick={handleNewPostButton}>New Post</button>
              </div>
          </div>
          <div id="body">

              <div id="home">
              </div>

              <div id="feeds">
              </div>

              <div id="account">
                  <button id="profilebannerbutton" onClick={handleEditBannerButton}>
                      <img className="profilebanner" src="/tmpbanner.jpeg" height="1500" width="500" id="accountbanner" />
                  </button>
                  <div className="account">
                      <button id="accountpfpbutton" onClick={handleEditPFPButton} className="editbutton">
                          <img src="https://upload.wikimedia.org/wikipedia/commons/a/ac/Default_pfp.jpg" width="128" height="128" className="profileimage" id="accountpfp" />
                      </button>
                    <div id="accountname" className="profilename">
                          <button id="accountname" className="profilename" onClick={handleEditDescriptionButton}>Username</button>
                    </div>
                    <div id="accountdesc" className="profiledesc">
                          <button id="editaccountdesc" className="editbutton" onClick={handleEditDescriptionButton}>Add a profile description!</button>
                    </div>
                      <div id="followeramt" className="following">
                          <h3>Follower</h3>
                          <h3 id="followingnumber" className="following">0</h3>
                    </div>
                      <div id="followingamt" className="following">
                      <h3>Following</h3>
                          <h3 id="following" className="following">0</h3>
                      </div>
                  </div>
              </div>

              <div id="newpost">
              </div>

              <div id="foreignaccount">
              </div>

          </div>

          <div id="posts">



          </div>


          <div id="testdivs">

              <div id="posttest" className="post">
                  <div id="posterinfodiv" className="posterinfo">
                      <button className="posterinfo">
                          <img src="https://upload.wikimedia.org/wikipedia/commons/a/ac/Default_pfp.jpg" width="64" height="64" className="profileimage" />
                          <h3 className="postername">Username</h3>
                          <h4 className="posterdate">Post Date</h4>
                          <h4 className="postcommentamount">0</h4>
                          <h4 className="postlikeamount">0</h4>
                          <h4 className="postrepostamount">0</h4>
                      </button>
                  </div>
                  <div id="postcontent" className="postcontent">
                    <h4 id="postcontenttext" className="postcontenttext">Temp</h4>
                    <img src="" id="postcontentimg" className="postcontentimg"/>
                  </div>
              </div>

          </div>

          <div id="popups">
              <div id="editprofiledesc" className="modal">
                <div className="modalcontent">
                  <form id="editdescform" onSubmit={handleDescInputSubmit}>
                        <textarea
                          form="editdescform"
                          id="desctextarea"
                          placeholder="Add your new description here!"
                          className="inputfield"
                          maxLength={500}
                          rows={10}
                          cols={40}
                          autoCorrect="on"
                          spellCheck="true"
                        />
                    <button type="submit" className="modalbutton">Submit</button>
                  </form>
                </div>
              </div>

              <div id="editpfp" className="modal">
                <div className="modalcontent">
                      <input type="file" onChange={onFileChangePFP} accept="image/jpeg, image/png" className="fileuploadfield" />
                      <button onClick={handlePFPImageFileUpload} className="fileuploadbutton">Submit</button>
                </div>
              </div>
              <div id="editbanner" className="modal">
                  <div className="modalcontent">
                      <input type="file" onChange={onFileChangeBanner} accept="image/jpeg, image/png" className="fileuploadfield" />
                      <button onClick={handleProfileBannerImageUpload} className="fileuploadbutton">Submit</button>
                  </div>
              </div>

              <div id="newposteditor" className="modal">
                  <div className="modalcontent">
                      <form id="newpostform" onSubmit={handleNewPostSubmit}>
                          <textarea
                              form="newpostform"
                              id="newposttextarea"
                              placeholder="What to talk about?"
                              className="inputfield"
                              maxLength={500}
                              rows={10}
                              cols={40}
                              spellCheck="true"
                          />
                          <input type="file" onChange={onFileChangePostImage} accept="image/jpeg, image/png" className="fileuploadfield" />
                          <button type="submit" className="modalbutton">Submit</button>
                      </form>
                  </div>
              </div>
          </div>
          
      </main>
    );
}

export default home;