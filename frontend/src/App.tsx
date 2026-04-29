import { useState} from 'react'
import aspireLogo from '/Aspire.png'
import './App.css'

interface UserInfo {
    uid: number
    token: string
}
interface FormElements extends HTMLFormControlsCollection {
    usernameInput: HTMLInputElement
    passwordInput: HTMLInputElement
}

interface FormElementsCreateUser extends HTMLFormControlsCollection {
    usernameInput: HTMLInputElement
    passwordInput: HTMLInputElement
    passwordConfirmInput: HTMLInputElement
}
interface LoginFormElement extends HTMLFormElement {
    readonly elements: FormElements
}

interface CreateUserFormElement extends HTMLFormElement {
    readonly elements: FormElementsCreateUser
}

function App() {
  const [error, setError] = useState<string | null>(null)


    const fetchLoginQuery = async (username: string, password: string) => {
        setError(null)

        const userPass = username + ":" + password;
        try {  
            const response = await fetch('api/mimirpostgreslogin?userAndPass=' + userPass.toString(), {
                method: "POST",
            })

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`)
            }

            const data: UserInfo[] = await response.json()
            if (data[0].toString() === "0") {
                setError("Invalid Username/Password");
                return;
            }
            localStorage.setItem("uid", data[0].toString());
            localStorage.setItem("sessionid", data[1].toString())
            window.location.assign("/home/home.html");
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to log in')
            console.error('Error loggin in:', err)
        }
    }

    const fetchCreateUserQuery = async (username: string, password: string, passwordCon: string) =>
    {
        if (password === passwordCon) {
            const userPass = username + ":" + password;
            try {
                const response = await fetch('api/mimirpostgrescreateuser?userAndPass=' + userPass.toString(), {
                    method: "POST"
                })
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`)
                }
                const data: UserInfo[] = await response.json()
                alert("New session ID = " + data[0].toString() + " " + data[1].toString())
                localStorage.setItem("uid", data[0].toString());
                localStorage.setItem("sessionid", data[1].toString())
                location.assign("/home/home.html");
            }
            catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to log in')
            console.error('Error loggin in:', err)
            }
        }
    }
  function handleLoginSubmit(event: React.FormEvent<LoginFormElement>) {
      event.preventDefault()
      fetchLoginQuery(event.currentTarget.elements.usernameInput.value, event.currentTarget.elements.passwordInput.value)
    }

    function handleCreateUserSubmit(event: React.FormEvent<CreateUserFormElement>) {
        event.preventDefault()
        fetchCreateUserQuery(event.currentTarget.elements.usernameInput.value, event.currentTarget.elements.passwordInput.value, event.currentTarget.elements.passwordConfirmInput.value)
    }

    function toggleShowElement(elementName: string) {
        const x = document.getElementById(elementName);
        if (x.style.display === "none") {
            x.style.display = "block";
            if (elementName === "loginDiv") {
                const z = document.getElementById("createUserDiv");
                z.style.display = "none";
            }
            if (elementName === "createUserDiv") {
                const z = document.getElementById("loginDiv");
                z.style.display = "none";
            }
        } else {
            x.style.display = "none";
            if (elementName === "loginDiv") {
                const z = document.getElementById("createUserDiv");
                z.style.display = "block";
            }
            if (elementName === "createUserDiv") {
                const z = document.getElementById("loginDiv");
                z.style.display = "block";
            }
        }
    }

    function handleButtonEvent(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault()
        toggleShowElement(event.currentTarget.value)
    }

  return (
    <div className="app-container">
      <header className="app-header">
        <a 
          href="https://aspire.dev" 
          target="_blank" 
          rel="noopener noreferrer"
          aria-label="Visit Aspire website (opens in new tab)"
          className="logo-link"
        >
          <img src={aspireLogo} className="logo" alt="Aspire logo" />
        </a>
        <h1 className="app-title">Log In</h1>
      </header>

      <main className="main-content">
              <section className="login-section" aria-labelledby="">
                  <button id="loginDivButton" value="loginDiv" onClick={handleButtonEvent}>Login</button>
                  <button id="createUserDivButton" value="createUserDiv" onClick={handleButtonEvent}>New User</button>
            <div className="loginform" id="loginDiv">
                      <form onSubmit={handleLoginSubmit}>
                          <h3> Log In </h3>
                          <div>
                              <label htmlFor="usernameInput">
                                  Username:
                              </label>
                              <input type="text" id="usernameInput" />
                          </div>
                          <div>
                              <label htmlFor="passwordInput">
                                  Password:
                              </label>
                              <input type="text" id="passwordInput" />
                              
                          </div>
                          <div>
                              <input type="submit" value="Login" />
                          </div>
                  </form>
                  </div>

                  <div className="createuserform" id="createUserDiv">
                      <form onSubmit={handleCreateUserSubmit}>
                          <h3> Create New User </h3>
                          <div>
                              <label htmlFor="usernameInput">
                                  Username:
                              </label>
                              <input type="text" id="usernameInput" />
                          </div>
                          <div>
                              <label htmlFor="passwordInput">
                                  Password:
                              </label>
                              <input type="text" id="passwordInput" />

                          </div>
                          <div>
                              <label htmlFor="passwordConfirmInput">
                                  Password Confirm:
                              </label>
                              <input type="text" id="passwordConfirmInput" />

                          </div>
                          <div>
                              <input type="submit" value="CreateUser" />
                          </div>
                      </form>
                  </div>
        </section>
        
            {error && (
              <div className="error-message" role="alert" aria-live="polite">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" aria-hidden="true">
                  <circle cx="12" cy="12" r="10"/>
                  <line x1="12" y1="8" x2="12" y2="12"/>
                  <line x1="12" y1="16" x2="12.01" y2="16"/>
                </svg>
                <span>{error}</span>
              </div>
            )}
      </main>

      <footer className="app-footer">
        <nav aria-label="Footer navigation">
          <a href="https://aspire.dev" target="_blank" rel="noopener noreferrer">
            Learn more about Aspire<span className="visually-hidden"> (opens in new tab)</span>
          </a>
          <a 
            href="https://github.com/dotnet/aspire" 
            target="_blank" 
            rel="noopener noreferrer"
            className="github-link"
            aria-label="View Aspire on GitHub (opens in new tab)"
          >
            <img src="/github.svg" alt="" width="24" height="24" aria-hidden="true" />
            <span className="visually-hidden">GitHub</span>
          </a>
        </nav>
      </footer>
    </div>
  )
}

export default App
