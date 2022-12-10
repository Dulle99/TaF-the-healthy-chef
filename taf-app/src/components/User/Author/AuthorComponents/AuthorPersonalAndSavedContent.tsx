import { Box, Button, Container, Typography } from "@mui/material";
import axios from "axios";
import { useEffect, useState } from "react";
import BlogPreview from "../../../Blog/BlogComponents/BlogPreview";
import IBlogPreviewProp from "../../../Blog/BlogInterfaces/IBlogPreviewProp";
import CookingRecepiePreview from "../../../CookingRecepie/CookingRecepieComponents/CookingRecepiePreview";
import ICookingRecepiePreviewProp from "../../../CookingRecepie/CookingRecepieInterface/ICookingRecepiePreviewProps";
import DeleteSavedContent from "../../MutalUserComponents/Components/DeleteSavedContent";
import UpdateAndDeleteButton from "../../MutalUserComponents/Components/UpdateAndDeleteButton";
import IAuthorProfileContentSelectPreview from "../AuthorInterface/IAuthorProfileContentSelectPreview";


function AuthorPersonalAndSavedContent(prop: IAuthorProfileContentSelectPreview) {
    const [authorsBlogs, setAuthorsBlogs] = useState<IBlogPreviewProp[]>([]);
    const [authorsCookingRecepies, setAuthorsCookingRecepies] = useState<ICookingRecepiePreviewProp[]>([]);
    const [authorsSavedBlogs, setAuthorSavedBlogs] = useState<IBlogPreviewProp[]>([]);
    const [authorsSavedCookingRecepies, setAuthorsSavedCookingRecepies] = useState<ICookingRecepiePreviewProp[]>([]);
    const [numberOfBlogsToGet, setNumberOfBlogsToGet] = useState(5);
    const [numberOfSavedBlogsToGet, setNumberOfSavedBlogsToGet] = useState(5);
    const [numberOfCookingRecepiesToGet, setnumberOfCookingRecepiesToGet] = useState(5);
    const [numberOfSavedCookingRecepiesToGet, setnumberOfSavedCookingRecepiesToGet] = useState(5);

    async function deleteContent(isCookingRecepie: boolean, contentId: string) {
        let username = sessionStorage.getItem('username');
        let typeOfUser = sessionStorage.getItem('typeOfUser');
        if (username != null || typeOfUser != "Author") {
            let url: string = isCookingRecepie ?
                `https://localhost:5001/api/CookingRecepie/Delete/${contentId}`
                :
                `https://localhost:5001/api/Blog/Delete/${contentId}`;

            const result = await axios.delete(url, {
                headers: {
                    'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                },
            });

            if (result.status === 200) {
                if (isCookingRecepie) {
                    fetchCookingRecepies(numberOfSavedCookingRecepiesToGet);
                }
                else {
                    fetchBlogs(numberOfSavedBlogsToGet);
                }
            }

        }
    }

    async function removeContentFromReadLater(isCookingRecepie: boolean, contentId: string) {
        let username = sessionStorage.getItem('username');
        let typeOfUser = sessionStorage.getItem('typeOfUser');
        if (username != null || typeOfUser != null) {
            let url: string = isCookingRecepie ?
                `https://localhost:5001/api/User/RemoveCookingRecepieForReadLater/${contentId}/${username}/${typeOfUser}`
                :
                `https://localhost:5001/api/User/RemoveBlogForReadLater/${contentId}/${username}/${typeOfUser}`;

            await axios.delete(url, {
                headers: {
                    'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                },
            });

            if (isCookingRecepie) {
                fetchSavedCookingRecepies(numberOfSavedCookingRecepiesToGet);
            }
            else {
                fetchSavedBlogs(numberOfSavedBlogsToGet);
            }

        }
    }

    async function fetchBlogs(numberOfBlogsToGet: number) {
        const result = await axios.get<IBlogPreviewProp[]>(`https://localhost:5001/api/Blog/GetBlogsByAuthor/${prop.authorUsername}/${numberOfBlogsToGet.toString()}`,
            {
                headers: {
                    'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                },
            })
        if (result.status === 200) {
            setAuthorsBlogs(result.data);
        }
    }

    async function fetchCookingRecepies(numberOfBlogsToGet: number) {
        const result = await axios.get<ICookingRecepiePreviewProp[]>(`https://localhost:5001/api/CookingRecepie/GetCookingRecepiesByAuthor/${prop.authorUsername}/${numberOfBlogsToGet.toString()}`,
            {
                headers: {
                    'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                },
            });
        if (result.status === 200) {
            setAuthorsCookingRecepies(result.data);
        }
    }

    async function fetchSavedBlogs(numberOfSavedBlogsToGet: number) {
        const result = await axios.get<IBlogPreviewProp[]>(`https://localhost:5001/api/User/GetSavedBlogs/${prop.authorUsername}/Author/${numberOfSavedBlogsToGet}`,
            {
                headers: {
                    'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                },
            });
        if (result.status === 200) {
            setAuthorSavedBlogs(result.data);
        }
    }

    async function fetchSavedCookingRecepies(numberOfSavedRecepiesToGet: number) {
        const result = await axios.get<ICookingRecepiePreviewProp[]>(`https://localhost:5001/api/User/GetSavedCookingRecepie/${prop.authorUsername}/Author/${numberOfSavedRecepiesToGet}`,
            {
                headers: {
                    'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                },
            });
        if (result.status === 200) {
            setAuthorsSavedCookingRecepies(result.data);
        }
    }

    const handleClickLoadMoreCookingRecepies = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        fetchCookingRecepies(numberOfCookingRecepiesToGet + 5);
        setnumberOfCookingRecepiesToGet(numberOfCookingRecepiesToGet + 5);
    }

    const handleClickLoadMoreSavedCookingRecepies = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        fetchSavedCookingRecepies(numberOfSavedCookingRecepiesToGet + 5);
        setnumberOfSavedCookingRecepiesToGet(numberOfSavedCookingRecepiesToGet + 5);
    }

    const handleClickLoadMoreBlogs = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        fetchBlogs(numberOfBlogsToGet + 5);
        setNumberOfBlogsToGet(numberOfBlogsToGet + 5);
    }
    const handleClickLoadMoreSavedBlogs = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        fetchSavedBlogs(numberOfSavedBlogsToGet + 5);
        setNumberOfSavedBlogsToGet(numberOfSavedBlogsToGet + 5);
    }


    useEffect(() => {
        console.log(prop.contentType);
        if (prop.contentType === "cookingRecepies")
            fetchCookingRecepies(5);
        else if (prop.contentType === "blogs")
            fetchBlogs(5);
        else if (prop.contentType === "savedCookingRecepies")
            fetchSavedCookingRecepies(5);
        else if (prop.contentType === "savedBlogs")
            fetchSavedBlogs(5);
    }, [prop.contentType])

    if (prop.contentType === "cookingRecepies")
        return (<>
            <Container>
                <Typography variant="h5">Recepti</Typography>
                {authorsCookingRecepies.map((recepie, ind) => (
                    <Box sx={{ display: "flex", flexDirection: "column", mt: 2 }} key={recepie.cookingRecepieId + ind}>
                        <UpdateAndDeleteButton key={recepie.cookingRecepieId} typeOfContent={"cookingRecepies"} contentId={recepie.cookingRecepieId}
                            deleteContent={deleteContent} />
                        <CookingRecepiePreview {...recepie} key={ind} />
                    </Box>
                ))}

                <Box style={{ display: "flex", justifyContent: "center" }}>
                    {authorsCookingRecepies != null && authorsCookingRecepies.length > 0 ?
                        <Button
                            onClick={handleClickLoadMoreCookingRecepies}
                            size='small' sx={{
                                background: '#92279A', margin: '3px', ':hover': {
                                    bgcolor: '#b731c1',
                                    color: 'FFFFFF',
                                },
                            }}
                        >
                            <Typography color='#FFFFFF'>Učitaj još</Typography>
                        </Button>
                        :
                        <Typography color="gray">Nemate publikovane recepte.</Typography>}
                </Box>
            </Container>
        </>);

    else if (prop.contentType === "blogs")
        return (<>
            <Container>
                <Typography variant="h5">Blogovi</Typography>
                {authorsBlogs.map((blog, ind) => (
                    <Box sx={{ display: "flex", flexDirection: "column", mt: 2 }} key={blog.blogId + ind}>
                        <UpdateAndDeleteButton key={ind} typeOfContent={"blogs"} contentId={blog.blogId}
                            deleteContent={deleteContent} />
                        <BlogPreview {...blog} key={blog.blogId} />
                    </Box>
                ))}
                <Box style={{ display: "flex", justifyContent: "center" }}>
                    {authorsBlogs != null && authorsBlogs.length > 0 ?
                        <Button
                            onClick={handleClickLoadMoreBlogs}
                            size='small' sx={{
                                background: '#92279A', margin: '3px', ':hover': {
                                    bgcolor: '#b731c1',
                                    color: 'FFFFFF',
                                },
                            }}
                        >
                            <Typography color='#FFFFFF'>Učitaj još</Typography>
                        </Button>
                        :
                        <Typography color="gray">Nemate publikovane blogove.</Typography>}
                </Box>
            </Container>
        </>);

    else if (prop.contentType === "savedCookingRecepies")
        return (<>
            <Container>
                <Typography variant="h5">Sačuvani recepti</Typography>
                {authorsSavedCookingRecepies.map((recepie, ind) => (
                    <Box sx={{ display: "flex", flexDirection: "column", mt: 2 }} key={recepie.cookingRecepieId + ind}>
                        <Box sx={{ display: "flex", flexDirection: "row", justifyContent: "flex-end", mt: 1, mb: 1 }}>
                            <DeleteSavedContent typeOfContent="savedCookingRecepies" contentId={recepie.cookingRecepieId}
                                removeContentFromReadLater={removeContentFromReadLater} />
                        </Box>
                        <CookingRecepiePreview {...recepie} key={ind} />
                    </Box>
                ))}

                <Box style={{ display: "flex", justifyContent: "center" }}>
                    {authorsSavedCookingRecepies != null && authorsSavedCookingRecepies.length > 0 ?
                        <Button
                            onClick={handleClickLoadMoreSavedCookingRecepies}
                            size='small' sx={{
                                background: '#92279A', margin: '3px', ':hover': {
                                    bgcolor: '#b731c1',
                                    color: 'FFFFFF',
                                },
                            }}
                        >
                            <Typography color='#FFFFFF'>Učitaj još</Typography>
                        </Button>
                        :
                        <Typography color="gray">Nemate sačuvane recepte.</Typography>}
                </Box>
            </Container>
        </>);

    else
        return (<>
            <Container>
                <Typography variant="h5">Sačuvani blogovi</Typography>
                {authorsSavedBlogs.map((blog, ind) => (
                    <Box sx={{ display: "flex", flexDirection: "column", mt: 2 }} key={blog.blogId + ind}>
                        <Box sx={{ display: "flex", flexDirection: "row", justifyContent: "flex-end", mt: 1, mb: 1 }}>
                            <DeleteSavedContent typeOfContent="savedBlogs" contentId={blog.blogId}
                                removeContentFromReadLater={removeContentFromReadLater} />
                        </Box>
                        <BlogPreview {...blog} key={blog.blogId} />
                    </Box>
                ))}
                <Box style={{ display: "flex", justifyContent: "center" }}>
                    {authorsSavedBlogs != null && authorsSavedBlogs.length > 0 ?
                        <Button
                            onClick={handleClickLoadMoreSavedBlogs}
                            size='small' sx={{
                                background: '#92279A', margin: '3px', ':hover': {
                                    bgcolor: '#b731c1',
                                    color: 'FFFFFF',
                                },
                            }}
                        >
                            <Typography color='#FFFFFF'>Učitaj još</Typography>
                        </Button>
                        :
                        <Typography color="gray">Nemate sačuvane blogove.</Typography>}
                </Box>
            </Container>
        </>);
}
export default AuthorPersonalAndSavedContent;