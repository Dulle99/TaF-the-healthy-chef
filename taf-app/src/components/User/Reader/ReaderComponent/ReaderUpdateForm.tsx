import {
    Alert, AlertTitle, Button, Container, CssBaseline, FormControl, FormControlLabel, FormLabel, Grid, Radio, RadioGroup, TextField, Typography
} from "@mui/material";
import { Box } from "@mui/system";
import axios from "axios";
import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom"; import FormatDate from "../../../ContentMutalComponents/FormatDateFunction";
import IReader from "../ReaderInterface/IReader";
import IReaderUsername from "../ReaderInterface/IReaderUsername";

function ReaderUpdateForm() {

    const navigate = useNavigate();
    const location = useLocation().state as IReaderUsername;
    const [name, setName] = useState("");
    const [lastname, setLastname] = useState("");
    const [username, setUsername] = useState("");
    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [isFieldEmpthy, setIsFieldEmpthy] = useState(false);
    const [readerProfilePicturePreview, setReaderProfilePicturePreview] = useState<Blob>(new Blob);
    const [currentProfilePicture, setCurrentProfilePicture] = useState("");
    const [formProfilePicture, setFormProfilePicture] = useState<FormData>();
    const [usernameTaken, setUsernameTaken] = useState(false);
    const [badRequest, setBadRequest] = useState(false);
    const [schoolName, setSchoolName] = useState("");
    const [typeOfStudent, setTypeOfStudent] = useState("Student");


    function areFieldsEmpthy(): boolean {
        console.log(readerProfilePicturePreview);
        if (name === "" || lastname === "" || username === "" || currentPassword === "" || schoolName ==="") {
            setIsFieldEmpthy(true);
            return true;
        }
        else {
            setIsFieldEmpthy(false);
            return false;
        }
    }


    const updateUser = (formData: FormData) => {
        const url = `https://localhost:5001/api/User/UpdateReader/${location.username}`;
        axios.put(url, formData,
            {
                headers: {
                    'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                },
            })
            .then((response) => {
                if (response.status === 200) {
                    if (readerProfilePicturePreview != undefined) {
                        addProfilePicture();
                    }
                    window.sessionStorage.removeItem('username');
                    window.sessionStorage.setItem("username", username);
                    navigate('/homepage');
                }
            })
            .catch(() => {
                setBadRequest(true);
            });

    }

    async function addProfilePicture() {
        if (formProfilePicture != undefined) {
            formProfilePicture.append('username', username);
            formProfilePicture.append('typeOfUser', "Reader");
            const result = await axios.put('https://localhost:5001/api/User/UpdateProfilePicture', formProfilePicture,
                {
                    headers: {
                        'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                    },
                });

            if(result.status === 200){
                axios.get(`https://localhost:5001/api/User/GetUserProfilePicture/${sessionStorage.getItem("username")}/Reader`,
                        {
                            headers: {
                                'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                            },
                        })
                        .then((response) => {
                            if(response.status === 200){
                                window.sessionStorage.removeItem("profilePicture");
                                window.sessionStorage.setItem("profilePicture", response.data);
                            }
                        });
            }
        }
    }

    const handleNameChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setName(e.currentTarget.value);
    }

    const handleLastnameChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setLastname(e.currentTarget.value);
    }

    const handleUsernameChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setUsername(e.currentTarget.value);

    }

    const handleOldPasswordChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setCurrentPassword(e.currentTarget.value);
    }

    const handleNewPasswordChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setNewPassword(e.currentTarget.value);
    }

    const handleSchoolNameChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setSchoolName(e.currentTarget.value);
    }

    const handleTypeOfReaderChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        if((event.target as HTMLInputElement).value === "Student"){
            setTypeOfStudent("Student");
        }
        else{
            setTypeOfStudent("Srednjoškolac");
        }
    }

    const handleChangePicture: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        e.preventDefault();
        if (e.target.files !== null) {
            setReaderProfilePicturePreview(e.target.files[0]);
            let form = new FormData();
            for (let i = 0; i < e.target.files.length; i++) {
                let element = e.target.files[i];
                form.append('profilePicture', element);
            }
            setFormProfilePicture(form);
        }
    }

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (areFieldsEmpthy() === false) {
            const formData = new FormData();
            formData.append("name", name);
            formData.append("lastname", lastname);
            formData.append("username", username);
            formData.append("currentPassword", currentPassword);
            formData.append("newPassword", newPassword);
            formData.append("nameOfSchool", schoolName);
            formData.append("typeOfStudent", typeOfStudent)
            updateUser(formData);
        }


    };

    useEffect(() => {
        const fetchReader = async () => {
            const result = await axios.get<IReader>(`https://localhost:5001/api/User/GetUserPreview/${location.username}/Reader`,
                {
                    headers: {
                        'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                    },
                });
            if (result.status === 200) {
                setName(result.data.name);
                setLastname(result.data.lastname);
                setUsername(result.data.username);
                setSchoolName(result.data.nameOfSchool);
                setTypeOfStudent(result.data.typeOfStudent);
                setCurrentProfilePicture(result.data.profilePicture);
            }

        }

        fetchReader();

    }, []);

    useEffect(()=>{
        document.title = "Ažuriranje profila";
    }, []);

    const handlersForFirstConfig: React.ChangeEventHandler<HTMLInputElement>[] = [handleNameChange, handleLastnameChange];
    const handlersForSecondConfig: React.ChangeEventHandler<HTMLInputElement>[] = [handleUsernameChange, handleOldPasswordChange,
        handleNewPasswordChange, handleSchoolNameChange];
    const firstConfigs: string[][] = [["Ime", "Ime", "ime"], ["Prezime", "Prezime", "prezime"]];
    const secondConfigs: string[][] = [["Korisničko ime", "KorisnickoIme", "korisnickoIme", "text", username],
    ["Lozinka", "Lozinka", "Lozinka", "password", currentPassword],
    ["Nova lozinka", "Nova lozinka", "novaLozinka", "password", newPassword],
    ["Naziv škole", "Naziv škole", "nazivŠkole", "text", schoolName]];
    return (<>
        <Container component="main" maxWidth="xs">
            <CssBaseline />
            <Box
                sx={{
                    marginTop: 8,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                }}
            >
                <Typography component="h1" variant="h4"> Ažuriranje naloga </Typography>

                <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 3 }}>
                    <Grid container spacing={2}>
                        {firstConfigs.map((conf, ind) => (
                            <Grid item xs={12} sm={6} key={conf[0]}>
                                <TextField
                                    label={conf[0]}
                                    name={conf[1]}
                                    id={conf[2]}
                                    value={conf[1] === "Ime" ? name : lastname}
                                    required
                                    fullWidth
                                    autoFocus={conf[1] === "Ime" ? true : false}
                                    onChange={handlersForFirstConfig[ind]}
                                />
                            </Grid>
                        ))}

                        {secondConfigs.map((conf, ind) => (
                            <Grid item xs={12} key={conf[2]}>
                                <TextField
                                    label={conf[0]}
                                    name={conf[1]}
                                    id={conf[2]}
                                    type={conf[3]}
                                    value={conf[4]}
                                    required
                                    fullWidth
                                    multiline={conf[2] === "biografija" ? true : false}
                                    onChange={handlersForSecondConfig[ind]}
                                />
                            </Grid>
                        ))}

                        <FormControl style={{ margin: '20px' }}>
                            <FormLabel id="demo-controlled-radio-buttons-group">Vrsta čitaoca</FormLabel>
                            <RadioGroup
                                aria-labelledby="demo-controlled-radio-buttons-group"
                                name="controlled-radio-buttons-group"
                                value={typeOfStudent}
                                onChange={handleTypeOfReaderChange}
                                style={{ flexDirection: 'row', margin: '10px' }}

                            >
                                <FormControlLabel value="Student" control={<Radio />} label="Student" />
                                <FormControlLabel value="Srednjoškolac" control={<Radio />} label="Srednjoškolac" />
                            </RadioGroup>
                        </FormControl>

                        <Container sx={{ display: "flex", flexDirection: "column" }}>
                            <Box>
                                <Button
                                    variant="contained"
                                    component="label"
                                    sx={{
                                        mt: 1, mb: 1, background: '#2f9a27', ':hover': {
                                            bgcolor: '#27699A',
                                            color: 'FFFFFF',
                                        },
                                    }}
                                >
                                    Izaberi profilnu sliku
                                    <input
                                        type="file"
                                        hidden
                                        onChange={handleChangePicture}
                                    />
                                </Button>
                            </Box>
                            {readerProfilePicturePreview != null && readerProfilePicturePreview.size > 0 ?
                                <Box
                                    component="img"
                                    sx={{
                                        height: 233,
                                        width: 350,
                                        maxHeight: { xs: 233, md: 167 },
                                        maxWidth: { xs: 350, md: 250 },
                                    }}

                                    src={URL.createObjectURL(readerProfilePicturePreview)}
                                />
                                :
                                <Box
                                    component="img"
                                    sx={{
                                        height: 233,
                                        width: 350,
                                        maxHeight: { xs: 233, md: 167 },
                                        maxWidth: { xs: 350, md: 250 },
                                    }}

                                    src={currentProfilePicture.length > 0 ? `data:image/jpeg;base64,${currentProfilePicture}` : "/placeholderImage.jpg"}
                                />}

                        </Container>
                    </Grid>

                    {isFieldEmpthy === true ?
                        <Alert severity="warning" sx={{ mt: 2 }}>
                            <AlertTitle>Upozorenje</AlertTitle>
                            Morate popuniti sva relevantna polja! - <strong>morate uneti trenutnu lozinku kako bi potvrdili ažuriranje.</strong>
                        </Alert>
                        : ""}

                    {badRequest ?
                        <Alert severity="error" sx={{ mt: 2 }}>
                            <AlertTitle></AlertTitle>
                            Proverite unete podatke! - <strong>morate uneti trenutnu lozinku kako bi potvrdili ažuriranje.</strong>
                        </Alert>
                        : ""}

                    <Button

                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{
                            mt: 3, mb: 2, background: '#2f9a27', ':hover': {
                                bgcolor: '#27699A',
                                color: 'FFFFFF',
                            },
                        }}

                    >
                        Ažuriraj nalog
                    </Button>
                </Box>
            </Box>
        </Container>
    </>);
}
export default ReaderUpdateForm;