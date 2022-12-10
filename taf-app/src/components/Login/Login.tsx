import { Password, WindowSharp } from "@mui/icons-material";
import { Alert, AlertTitle, Button, Container, CssBaseline, Divider, TextField, Typography } from "@mui/material";
import { Box } from "@mui/system";
import axios from "axios";
import { useEffect, useState } from "react";
import { Navigate, Link, useLocation, useNavigate } from "react-router-dom";
import IUserLoggedStatusChange from "../IUserLoggedInvoke";
import LoginRegisterResponse from "./LoginRegisterResponse";

function Login(prop: IUserLoggedStatusChange) {
    let navigate = useNavigate();
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [invalidCredentialsFlag, setInvalidCredentialsFlag] = useState(false);
    const [isLoginSuccesful, setIsLoginSuccesful] = useState(false);


    const handleUsernameChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setUsername(e.currentTarget.value);

    }

    const handlePasswordChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setPassword(e.currentTarget.value);
    }

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (username.length === 0 && password.length === 0)
            setInvalidCredentialsFlag(true);
        else {
            setInvalidCredentialsFlag(false);
            const formData = new FormData();
            formData.append("username", username);
            formData.append("password", password);
            axios.post<LoginRegisterResponse>('https://localhost:5001/api/User/Login', formData)
                .then((response) => {
                    if (response.status === 200) {
                        setInvalidCredentialsFlag(false);
                        window.sessionStorage.setItem("token", response.data.token);
                        window.sessionStorage.setItem("username", response.data.username);
                        window.sessionStorage.setItem("typeOfUser", response.data.typeOfUser);
                        window.sessionStorage.setItem("isUserLogged", "true");

                        axios.get(`https://localhost:5001/api/User/GetUserProfilePicture/${response.data.username}/${response.data.typeOfUser}`,
                        {
                            headers: {
                                'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                            },
                        })
                        .then((response) => {
                            if(response.status === 200){
                                window.sessionStorage.setItem("profilePicture", response.data);
                            }
                        });
                       
                        setIsLoginSuccesful(true);
                        navigate(-1);
                        prop.userLogged();
                    }
                })
                .catch((err) => { setInvalidCredentialsFlag(true); });
        }
    };

    useEffect(()=>{
        document.title = "Prijavljivanje";
    }, []);

    return (
        <Container component="main">
            {isLoginSuccesful ? <Navigate to={"/homepage"} /> : ""}
            <CssBaseline />
            <Box
                sx={{
                    marginTop: 8,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                }}
            >
                <Typography variant="h2">Dobrodošli nazad na TaF</Typography>

                <Box component="form" onSubmit={handleSubmit} noValidate sx={{ textAlign: 'center' }} >
                    <TextField
                        margin="normal"
                        required
                        fullWidth
                        id="korisnickoIme"
                        label="Korisnicko ime"
                        name="korisnickoIme"
                        autoComplete="Korisnicko ime"
                        autoFocus
                        onChange={handleUsernameChange}
                        color={username.length ? 'primary' : 'error'}
                    />

                    <TextField
                        margin="normal"
                        required
                        fullWidth
                        id="lozinka"
                        label="Lozinka"
                        name="lozinka"
                        type="password"
                        autoComplete="current-password"
                        value={password}
                        onChange={handlePasswordChange}
                        color={password.length ? 'primary' : 'error'}
                    />


                    {invalidCredentialsFlag === true ?
                        <Alert severity="warning">
                            <AlertTitle >Upozorenje</AlertTitle>
                            Korisnik sa unetim korisničkim imenom ili lozinkom ne postoji — <strong>proverite na moguće slovne greške</strong>
                        </Alert> : ""}


                    <Button
                        type="submit"
                        size="medium"
                        variant="contained"
                        sx={{
                            mt: 3, mb: 2, background: '#2f9a27', ':hover': {
                                bgcolor: '#27699A',
                                color: 'FFFFFF',
                            },
                        }}
                    >
                        Prijavite se
                    </Button>

                </Box>
            </Box>

            <Box sx={{
                marginTop: 4,
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center'
            }}>
                <Divider variant="fullWidth" flexItem />
                <Typography variant="h5"> Nemate nalog? Registrujte se sada!</Typography>
                <Link to='/Register' style={{ textDecoration: 'none' }}>
                    <Button
                        type="submit"
                        size="medium"
                        variant="contained"
                        sx={{
                            mt: 3, mb: 2, textAlign: 'center', background: '#2f9a27', ':hover': {
                                bgcolor: '#27699A',
                                color: 'FFFFFF',
                            },
                        }}
                    >
                        Registrujte se
                    </Button>

                </Link>

            </Box>


            
        </Container>
    );
}
export default Login;