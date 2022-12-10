import { Alert, AlertTitle, Box, Button, Container, CssBaseline, Grid, TextField, Typography } from "@mui/material";
import axios from "axios";
import { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";
import IBlog from "../BlogInterfaces/IBlog";
import IBlogUpdate from "../BlogInterfaces/IBlogUpdate";
import placeholderImage from './placeholderImage.jpg';

function BlogForm(prop: IBlogUpdate | any) {
    const [blogTitle, setBlogTitle] = useState("");
    const [blogContent, setBlogContent] = useState("");
    const [readingTime, setReadingTime] = useState<string>("1");
    const [blogPicturePreview, setBlogPicturePreview] = useState<Blob>(new Blob);
    const [fetchedPictureForUpdate, setFetchedPictureForUpdate] = useState("");
    const [formData, setFormData] = useState<FormData>();
    const [blogCreated, setBlogCreatedFlag] = useState(false);
    const [areFieldsEmpthy, setAreFieldsEmpthyFlag] = useState(false);

    function validateFields(): boolean {
        if (blogTitle === "" || blogContent === "" || readingTime === "" || parseInt(readingTime) === NaN || parseInt(readingTime) <= 0 || formData === undefined) {
            setAreFieldsEmpthyFlag(true);
            return true;
        }
        else {
            setAreFieldsEmpthyFlag(false);
            return false;
        }
    }

    const handleBlogTitleChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setBlogTitle(e.currentTarget.value);

    }
    const handleBlogContentChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setBlogContent(e.currentTarget.value);

    }
    const handleBlogReadingTimeChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        const numberValue = parseInt(e.currentTarget.value);
        if(numberValue != NaN){
            setReadingTime(e.currentTarget.value);
        }
        else{
            setReadingTime("");
        }

    }

    const handleChangePicture: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        e.preventDefault();
        if (e.target.files !== null) {
            setBlogPicturePreview(e.target.files[0]);
            let form = new FormData();
            for (let i = 0; i < e.target.files.length; i++) {
                let element = e.target.files[i];
                form.append('blogPicture', element);
            }
            setFormData(form);
        }

    }

    const handleCreateBlogClick: React.MouseEventHandler<HTMLButtonElement> = (e) => {
        console.log(readingTime);
        if (validateFields() === false) {
            let form = new FormData();
            if (formData != undefined) {
                form = formData;
            }
            form.append('blogTitle', blogTitle);
            form.append('blogContent', blogContent);
            form.append('readingTime', readingTime.toString());

            if (prop.blogUpdate === true) {
                axios.put(`https://localhost:5001/api/Blog/UpdateBlog/${prop.blogId}`, formData,
                    {
                        headers: {
                            'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                        },
                    })
                    .then((response) => {
                        if (response.status === 200) {
                            setBlogCreatedFlag(true);
                        }
                    })
                    .catch(() => { });

            }
            else {
                axios.post(`https://localhost:5001/api/Blog/CreateBlog/${sessionStorage.getItem('username')}`, formData,
                    {
                        headers: {
                            'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                        },
                    })
                    .then((response) => {
                        if (response.status === 200) {
                            setBlogCreatedFlag(true);
                        }
                    })
                    .catch(() => { });
            }
        }

    }

    useEffect(() => {
        if (prop.blogUpdate != undefined && prop.blogId != undefined) {
            const fetchCookingRecepie = async () => {
                const result = await axios.get<IBlog>(`https://localhost:5001/api/Blog/GetBlog/${prop.blogId}`);
                if (result.status === 200) {
                    setBlogTitle(result.data.blogTitle);
                    setBlogContent(result.data.content);
                    setReadingTime(result.data.readingTime.toString());
                    setFetchedPictureForUpdate(result.data.blogPicture);
                    let form = new FormData();
                    form.append('blogPicture', result.data.blogPicture);

                    setFormData(form);
                }
            }

            fetchCookingRecepie();
        }
    }, []);

    useEffect(() => {
        if (prop.blogUpdate != undefined && prop.blogId != undefined) {
            document.title = "Ažuriranje bloga";  
        }
        else{
            document.title = "Kreiranje bloga";  
        }
      }, []);

    const configs: string[][] = [["Naslov bloga", "blogTitle", "naslovBloga"], ["Sadržaj bloga", "blogContent", "sadrzajBloga"]];
    const readTimeConfigs: string[] = ["Vreme čitanja", "readingTime", "vremeCitanja", "number"];
    const handlersForSecondConfig: React.ChangeEventHandler<HTMLInputElement>[] = [handleBlogTitleChange, handleBlogContentChange];
    return (
        <>
            {blogCreated === true ? <Navigate to='/homepage' /> : ""}
            <Container component="main" maxWidth="md">
                <CssBaseline />
                <Box
                    sx={{
                        marginTop: 8,
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'flex-start',
                    }}
                >
                    <Typography component="h1" variant="h4"> {prop.blogUpdate != undefined ? "Ažuriranje bloga" : "Kreiranje bloga"}</Typography>
                </Box>


                <Box component="form" noValidate sx={{ mt: 3 }}>

                    <Grid container spacing={2}>
                        {configs.map((conf, ind) => (
                            <Grid item xs={12} key={conf[2]}>
                                <TextField
                                    label={conf[0]}
                                    name={conf[1]}
                                    id={conf[2]}
                                    type="text"
                                    multiline={conf[2] === "sadrzajBloga" ? true : false}
                                    value={conf[2] === "sadrzajBloga" ? blogContent : blogTitle}
                                    rows={6}
                                    required
                                    fullWidth
                                    onChange={handlersForSecondConfig[ind]}
                                />
                            </Grid>))}

                        <Grid item xs={12} sm={6}>
                            <TextField
                                label={readTimeConfigs[0]}
                                name={readTimeConfigs[1]}
                                id={readTimeConfigs[2]}
                                value={readingTime}
                                required
                                fullWidth
                                type={readTimeConfigs[3]}
                                onChange={handleBlogReadingTimeChange}
                            />
                        </Grid>
                    </Grid>
                </Box>

                <Box sx={{ display: "flex", justifyContent: "flex-start", flexDirection: "column" }}>
                    <Box>
                        <Button
                            variant="contained"
                            component="label"
                            sx={{
                                mt: 8, mb: 2, background: '#2f9a27', ':hover': {
                                    bgcolor: '#27699A',
                                    color: 'FFFFFF',
                                },
                            }}
                        >
                            Izaberi sliku bloga
                            <input
                                type="file"
                                hidden
                                onChange={handleChangePicture}
                            />
                        </Button>
                    </Box>
                    {blogPicturePreview != null && blogPicturePreview.size > 0 ?
                        <Box
                            component="img"
                            sx={{
                                height: 233,
                                width: 350,
                                maxHeight: { xs: 233, md: 167 },
                                maxWidth: { xs: 350, md: 250 },
                            }}

                            src={URL.createObjectURL(blogPicturePreview)}
                        />
                        :
                        <Box
                            component="img"
                            sx={{
                                height: 150,
                                width: 200,
                                maxHeight: { xs: 433, md: 367 },
                                maxWidth: { xs: 550, md: 450 },
                            }}
                            src={fetchedPictureForUpdate.length > 0 ?`data:image/jpeg;base64,${fetchedPictureForUpdate}` : '/placeholderImage.jpg'}
                        />}

                </Box>

                {areFieldsEmpthy === true ?
                    <Alert severity="warning" sx={{ mt: 2 }}>
                        <AlertTitle>Upozorenje</AlertTitle>
                        Morate popuniti sva relevantna polja i uneti sliku!
                    </Alert>
                    : ""}

                <Box style={{ display: "flex", justifyContent: "center" }}>
                    <Button
                        type="submit"
                        onClick={handleCreateBlogClick}
                        variant="contained"
                        sx={{
                            mt: 3, mb: 2, background: '#2f9a27', ':hover': {
                                bgcolor: '#27699A',
                                color: 'FFFFFF',
                            },

                        }}
                    >
                        {prop.blogUpdate === true ? "Azuriraj blog" : "Kreiraj blog"}
                    </Button>
                </Box>

            </Container>
        </>
    );
}
export default BlogForm;