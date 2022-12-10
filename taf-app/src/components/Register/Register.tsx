import {
    Alert, AlertTitle, Button, Container, CssBaseline, FormControl, FormControlLabel,
    FormLabel, Grid, Radio, RadioGroup, TextField, Typography
} from "@mui/material";
import { Box } from "@mui/system";
import axios from "axios";
import { useEffect, useState } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import IUserLoggedStatusChange from "../IUserLoggedInvoke";
import LoginRegisterResponse from "../Login/LoginRegisterResponse";
import AuthorAdditionalFields from "./AuthorFormAdditionalFields";
import IAward from "./IAward";
import ReaderAdditionalFields from "./ReaderFormAdditionalFields";


interface Award {
    awardName: string,
    dateOfReceivingAward: string
}

function Register(prop: IUserLoggedStatusChange) {
    let navigate = useNavigate();
    const [name, setName] = useState("");
    const [lastname, setLastname] = useState("");
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [typeOfUser, setTypeOfUser] = useState('Citalac');
    const [biography, setBiography] = useState("");
    const [YearsOfExpiriance, setYearsOfExpiriance] = useState(1);
    const [awards, setAwards] = useState<IAward[]>([]);
    const [schoolName, setSchoolName] = useState("");
    const [typeOfStudent, setTypeOfStudent] = useState("Student");
    const [isFieldEmpthy, setIsFieldEmpthy] = useState(false);
    const [isUserLogged, setIsUserLogged] = useState(false);
    const [cookingRecepiePicturePreview, setCookingRecepiePicturePreview] = useState<Blob>(new Blob);
    const [formProfilePicture, setFormProfilePicture] = useState<FormData>();
    const [usernameTaken, setUsernameTaken] = useState(false);

    function areFieldsEmpthy(): boolean {
        if (name === "" || lastname === "" || username === "" || password === "") {
             setIsFieldEmpthy(true);
             return true;
        }
        else {
            if (typeOfUser === "Citalac") {
                if (schoolName === "") {
                     setIsFieldEmpthy(true);
                     return true;
                }
            }
            else {
                if (biography === "") {
                    setIsFieldEmpthy(true);
                    return true;
                }
                else if (YearsOfExpiriance <= 0){
                    setIsFieldEmpthy(true);
                    return true;
                }
                else if (cookingRecepiePicturePreview === undefined){
                    setIsFieldEmpthy(true);
                    return true;
                }
            }
        }
        setIsFieldEmpthy(false);
        return false; 
    }

    const registerUser = (formData: FormData) => {
        let url: string = "";
        if (typeOfUser === "Citalac") {
            url = 'https://localhost:5001/api/User/RegisterReader';
        }
        else {
            url = 'https://localhost:5001/api/User/RegisterAuthor';
        }
        axios.post<LoginRegisterResponse>(url, formData)
            .then((response) => {
                if (response.status === 200) {
                    window.sessionStorage.setItem("token", response.data.token);
                    window.sessionStorage.setItem("username", response.data.username);
                    window.sessionStorage.setItem("typeOfUser", response.data.typeOfUser);
                    window.sessionStorage.setItem("isUserLogged", "true");
                    setIsUserLogged(true);
                    addProfilePicture();

                    axios.get(`https://localhost:5001/api/GetUserProfilePicture/${response.data.username}/${response.data.typeOfUser}`,
                        {
                            headers: {
                                'Authorization': 'Bearer ' + window.sessionStorage.getItem('token')
                            },
                        })
                        .then((response) => {
                            if(response.status === 200){
                                window.sessionStorage.setItem("profilePicture", response.data);
                            }
                        });
                    navigate('/homepage');
                    prop.userLogged();
                }
            })
            .catch((err) => { setUsernameTaken(true); });
    }

    const addProfilePicture = () => {
        if (formProfilePicture != undefined) {
            formProfilePicture.append('username', username);
            if (typeOfUser === "Citalac") { formProfilePicture.append('typeOfUser', "Reader"); }
            else { formProfilePicture.append('typeOfUser', "Author"); }
            axios.put('https://localhost:5001/api/User/UpdateProfilePicture', formProfilePicture,
                {
                    headers: {
                        'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                    },
                });
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

    const handlePasswordChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setPassword(e.currentTarget.value);
    }

    const handleTypeOfUserChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setTypeOfUser((event.target as HTMLInputElement).value);
    }

    const handleBiographyChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setBiography(e.currentTarget.value);
    }

    const handleYearsOfExpirianceChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setYearsOfExpiriance(Number(e.currentTarget.value));
    }

    function pushAwardToArray(award: IAward): void {
        if (award.awardName !== "" && award.dateOfReceivingAward.getDay() !== 0)
            setAwards([...awards, award]);
    }

    function removeAwardFromArray(awardIndex: number): void {
        setAwards((awards) => awards.filter((award, ind) => ind !== awardIndex));
    }

    const handleSchoolNameChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setSchoolName(e.currentTarget.value);
    }

    const handleTypeOfStudentChange: React.ChangeEventHandler<HTMLInputElement> = (event) => {
        if ((event.target as HTMLInputElement).value === "Student") {
            setTypeOfStudent("Student");
        }
        else {
            setTypeOfStudent("Reader");
        }
    }

    const handleChangePicture: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        e.preventDefault();
        if (e.target.files !== null) {
            setCookingRecepiePicturePreview(e.target.files[0]);
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
            formData.append("password", password);
            if (typeOfUser == "Citalac") {
                formData.append("nameOfSchool", schoolName);
                formData.append("typeOfStudent", typeOfStudent)
            }
            else {
                formData.append("biography", biography);
                formData.append("yearsOfExpiriance", YearsOfExpiriance.toString());
                awards.map((award, ind) => {
                    formData.append(`awards[${ind}].awardName`, award.awardName);
                    formData.append(`awards[${ind}].dateOfReceivingAward`, award.dateOfReceivingAward.toJSON());
                })
            }
            registerUser(formData);
        }
    };

    useEffect(()=>{
        document.title = "Kreiranje naloga";
    }, []);

    const handlersForFirstConfig: React.ChangeEventHandler<HTMLInputElement>[] = [handleNameChange, handleLastnameChange];
    const handlersForSecondConfig: React.ChangeEventHandler<HTMLInputElement>[] = [handleUsernameChange, handlePasswordChange];

    const firstConfigs: string[][] = [["Ime", "Ime", "ime"], ["Prezime", "Prezime", "prezime"]];
    const secondConfigs: string[][] = [["Korisničko ime", "KorisnickoIme", "korisnickoIme", "text"], ["Lozinka", "Lozinka", "lozinka", "password"]];
    return (
        <Container component="main" maxWidth="xs">
            {isUserLogged ? <Navigate to={"/homepage"} /> : ""}
            <CssBaseline />
            <Box
                sx={{
                    marginTop: 8,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                }}
            >
                <Typography component="h1" variant="h4"> Kreiraj TaF nalog  </Typography>

                <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 3 }}>
                    <Grid container spacing={2}>
                        {firstConfigs.map((conf, ind) => (
                            <Grid item xs={12} sm={6} key={conf[0]}>
                                <TextField
                                    label={conf[0]}
                                    name={conf[1]}
                                    id={conf[2]}
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
                                    required
                                    fullWidth
                                    onChange={handlersForSecondConfig[ind]}
                                />
                            </Grid>
                        ))}
                        <FormControl style={{ margin: '20px' }}>
                            <FormLabel id="demo-controlled-radio-buttons-group">Vrsta korisnika</FormLabel>
                            <RadioGroup
                                aria-labelledby="demo-controlled-radio-buttons-group"
                                name="controlled-radio-buttons-group"
                                value={typeOfUser}
                                onChange={handleTypeOfUserChange}
                                style={{ flexDirection: 'row', margin: '10px' }}
                            >
                                <FormControlLabel value="Citalac" control={<Radio />} label="Čitalac" />
                                <FormControlLabel value="Autor" control={<Radio />} label="Autor" />
                            </RadioGroup>
                        </FormControl>

                        {typeOfUser === "Citalac" ? <ReaderAdditionalFields handleSchoolNameChange={handleSchoolNameChange} handleTypeOfStudentChange={handleTypeOfStudentChange} />
                            : <AuthorAdditionalFields handleBiographyChange={handleBiographyChange} handleYearsOfExpirianceChange={handleYearsOfExpirianceChange}
                                pushAwardToArray={pushAwardToArray} removeAwardFromArray={removeAwardFromArray} awards={awards} />}

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
                            {cookingRecepiePicturePreview!= null && cookingRecepiePicturePreview.size > 0 ?
                                <Box
                                    component="img"
                                    sx={{
                                        height: 233,
                                        width: 350,
                                        maxHeight: { xs: 233, md: 167 },
                                        maxWidth: { xs: 350, md: 250 },
                                    }}

                                    src={URL.createObjectURL(cookingRecepiePicturePreview)}
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

                                src="/placeholderImage.jpg"
                            />  }

                        </Container>
                    </Grid>

                    {isFieldEmpthy === true ?
                        <Alert severity="warning" sx={{ mt: 2 }}>
                            <AlertTitle>Upozorenje</AlertTitle>
                            Morate popuniti sva relevantna polja!
                        </Alert>
                        : ""}

                    {usernameTaken ?
                        <Alert severity="error" sx={{ mt: 2 }}>
                            <AlertTitle></AlertTitle>
                            Uneto korisnicko ime je zauzeto!
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
                        Kreiraj nalog
                    </Button>
                </Box>
            </Box>
        </Container>
    );
}
export default Register;