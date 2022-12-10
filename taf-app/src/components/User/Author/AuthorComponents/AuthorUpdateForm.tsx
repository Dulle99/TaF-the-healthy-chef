import { Alert, AlertTitle, Button, Container, CssBaseline, Grid, TextField, Typography
} from "@mui/material";
import { Box } from "@mui/system";
import axios from "axios";
import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";import FormatDate from "../../../ContentMutalComponents/FormatDateFunction";
 import AuthorAwardComponent from "../../../Register/AuthorAwardComponent";
import IAward from "../../../Register/IAward";
import IAuthor from "../AuthorInterface/IAuthor";
import IAuthorUsername from "../AuthorInterface/IAuthorUsername";



function AuthorUpdateForm() {
    const navigate = useNavigate();
    const location = useLocation().state as IAuthorUsername;
    const [name, setName] = useState("");
    const [lastname, setLastname] = useState("");
    const [username, setUsername] = useState("");
    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [biography, setBiography] = useState("");
    const [YearsOfExpiriance, setYearsOfExpiriance] = useState(1);
    const [awards, setAwards] = useState<IAward[]>([]);
    const [isFieldEmpthy, setIsFieldEmpthy] = useState(false);
    const [isUserLogged, setIsUserLogged] = useState(false);
    const [authorProfilePicturePreview, setAuthorProfilePicturePreview] = useState<Blob>(new Blob);
    const [currentProfilePicture, setCurrentProfilePicture] = useState("");
    const [formProfilePicture, setFormProfilePicture] = useState<FormData>();
    const [usernameTaken, setUsernameTaken] = useState(false);
    const [badRequest, setBadRequest] = useState(false);


    function areFieldsEmpthy(): boolean {
        console.log(authorProfilePicturePreview);
        if (name === "" || lastname === "" || username === "" || currentPassword === "" ||
            biography === "" || YearsOfExpiriance <= 0 ) {
            setIsFieldEmpthy(true);
            return true;
        }
        else {
            setIsFieldEmpthy(false);
            return false;
        }
    }

    const buttonHandlerRemoveAward = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        console.log(event.currentTarget.id);
        removeAwardFromArray(parseInt(event.currentTarget.id, 10));
    }

    const updateUser = (formData: FormData) => {
           const url = `https://localhost:5001/api/User/UpdateAuthor/${location.username}`;
           axios.put(url, formData,
                    {
                        headers: {
                            'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                        },
                    })
                    .then((response) => {
                        if (response.status === 200) {
                            if(authorProfilePicturePreview != undefined){
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

    async function addProfilePicture(){
        if (formProfilePicture != undefined) {
            formProfilePicture.append('username', username);
            formProfilePicture.append('typeOfUser', "Author"); 
            const result = await axios.put('https://localhost:5001/api/User/UpdateProfilePicture', formProfilePicture,
                {
                    headers: {
                        'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                    },
                });

            if(result.status === 200){
                axios.get(`https://localhost:5001/api/User/GetUserProfilePicture/${window.sessionStorage.getItem("username")}/Author`,
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


    const handleBiographyChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setBiography(e.currentTarget.value);
    }

    const handleYearsOfExpirianceChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setYearsOfExpiriance(Number(e.currentTarget.value));
    }

    function pushAwardToArray(award: IAward): void {
        if (award.awardName !== "" && award.dateOfReceivingAward.getDay() !== 0)
        {
            setAwards([...awards, award]);
        }
    }

    function removeAwardFromArray(awardIndex: number): void {
        setAwards((awards) => awards.filter((award, ind) => ind !== awardIndex));
    }

    const handleChangePicture: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        e.preventDefault();
        if (e.target.files !== null) {
            setAuthorProfilePicturePreview(e.target.files[0]);
            let form = new FormData();
            for (let i = 0; i < e.target.files.length; i++) {
                let element = e.target.files[i];
                console.log(i);
                console.log(e.target.files[i]);
                form.append('profilePicture', element);
            }
            setFormProfilePicture(form);
        }
    }

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        console.log(awards);
        if (areFieldsEmpthy() === false) {
            console.log(awards);
            const formData = new FormData();
            formData.append("name", name);
            formData.append("lastname", lastname);
            formData.append("username", username);
            formData.append("currentPassword", currentPassword);
            formData.append("newPassword", newPassword);
            formData.append("biography", biography);
            formData.append("yearsOfExpiriance", YearsOfExpiriance.toString());
            awards.map((award, ind) => {
                formData.append(`awards[${ind}].awardName`, award.awardName);
                formData.append(`awards[${ind}].dateOfReceivingAward`, award.dateOfReceivingAward.toJSON());
            });
            updateUser(formData);
        }


    };

    useEffect(() => {


        const fetchAuthor = async () => {
            const result = await axios.get<IAuthor>(`https://localhost:5001/api/User/GetUserPreview/${location.username}/Author`,
                {
                    headers: {
                        'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                    },
                });
            if (result.status === 200) {
                setName(result.data.name);
                setLastname(result.data.lastname);
                setUsername(result.data.username);
                setBiography(result.data.biography);
                setYearsOfExpiriance(result.data.yearsOfExpiriance);
                setCurrentProfilePicture(result.data.profilePicture);
                let awardsDTO : IAward[] = [];
                result.data.awards.forEach  ((award, ind) =>{
                    awardsDTO.push({ awardName: award.awardName, dateOfReceivingAward: new Date(award.dateOfReceivingAward)});
                });
                setAwards(awardsDTO);
            }
            
        }

        fetchAuthor();


    }, []);

    useEffect(()=>{
        document.title = "Ažuriranje profila";
    }, []);

    const handlersForFirstConfig: React.ChangeEventHandler<HTMLInputElement>[] = [handleNameChange, handleLastnameChange];
    const handlersForSecondConfig: React.ChangeEventHandler<HTMLInputElement>[] = [handleUsernameChange, handleOldPasswordChange,
        handleNewPasswordChange, handleBiographyChange,
        handleYearsOfExpirianceChange];

    const firstConfigs: string[][] = [["Ime", "Ime", "ime"], ["Prezime", "Prezime", "prezime"]];
    const secondConfigs: string[][] = [["Korisničko ime", "KorisnickoIme", "korisnickoIme", "text", username],
    ["Lozinka", "Lozinka", "Lozinka", "password", currentPassword],
    ["Nova lozinka", "Nova lozinka", "novaLozinka", "password", newPassword],
    ["Biografija", "Biografija", "biografija", "text", biography],
    ["Godine iskustva", "GodineIskustva", "godineIskustva", "number", YearsOfExpiriance.toString()]];

    return (
        <>
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

                            <Grid >
                                <Typography sx={{ marginLeft: "30px", marginTop: "5px" }} variant="body1" position="relative">Nagrade:</Typography>

                                {awards.map((award, ind) => (
                                    <Box maxWidth="400px" sx={{ paddingLeft: "25px" }} key={ind}>
                                        <Typography > {ind + 1 + ". " + award.awardName + " " + FormatDate(award.dateOfReceivingAward)}

                                        </Typography>
                                        <Box>
                                            <Button id={ind.toString()}
                                                size="small"
                                                variant="contained"
                                                sx={{
                                                    background: '#e01f2a', ':hover': {
                                                        bgcolor: '#ed7c83',
                                                        color: 'FFFFFF',
                                                    }, marginLeft: "5px"
                                                }}
                                                onClick={buttonHandlerRemoveAward}>
                                                Obriši
                                            </Button>
                                        </Box>
                                    </Box>))}
                            </Grid>
                            <AuthorAwardComponent pushAwardToArray={pushAwardToArray} popAwardFromArray={removeAwardFromArray} />

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
                                {authorProfilePicturePreview != null && authorProfilePicturePreview.size > 0 ?
                                    <Box
                                        component="img"
                                        sx={{
                                            height: 233,
                                            width: 350,
                                            maxHeight: { xs: 233, md: 167 },
                                            maxWidth: { xs: 350, md: 250 },
                                        }}

                                        src={URL.createObjectURL(authorProfilePicturePreview)}
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
        </>
    );
}
export default AuthorUpdateForm;