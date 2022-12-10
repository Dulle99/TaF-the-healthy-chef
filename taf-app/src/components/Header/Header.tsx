import React, { useEffect, useState } from 'react';
import { Component, ReactNode, Fragment } from 'react';
import { Link, Outlet } from 'react-router-dom';
import { GiCook } from "react-icons/gi";

import { Toolbar, Container, Typography, Box, Button, Avatar, Icon, IconButton, Menu } from '@mui/material';
import AddCircleIcon from '@mui/icons-material/AddCircle';
import SettingsIcon from '@mui/icons-material/Settings';
import LogoutIcon from '@mui/icons-material/Logout';
import { border } from '@mui/system';
import HeaderButton from './Header-button';
import CookingRecepieDropdownmenu from './CookingRecepiesDropDownmenu';
import BlogPreviews from '../Blog/BlogComponents/BlogPreviews';
import ILoggedStatus from '../ILoggedStatus';
import IUserLoggedStatusChange from '../IUserLoggedInvoke';
import { AiOutlineMenu } from "react-icons/ai";


function Header(prop: IUserLoggedStatusChange) {
    const [token, setToken] = useState<string | null>("");
    const isUserLoggedIn: boolean = sessionStorage.getItem('isUserLogged') !== null;
    const isUserAuthor: boolean = sessionStorage.getItem('typeOfUser') == "Author";
    const [anchorElNav, setAnchorElNav] = useState<null | HTMLElement>(null);
    const [anchorElUser, setAnchorElUser] = useState<null | HTMLElement>(null);

    const handleOpenNavMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorElNav(event.currentTarget);
    };

    const handleCloseNavMenu = () => {
        setAnchorElNav(null);
    };

    const handleLogout: React.MouseEventHandler<HTMLButtonElement> = (e) => {
        sessionStorage.removeItem('token');
        sessionStorage.removeItem('username');
        sessionStorage.removeItem('typeOfUser');
        sessionStorage.removeItem('isUserLogged');
        sessionStorage.removeItem('profilePicture');
        setToken(null);
        prop.userLogged();
    }

    useEffect(() => {
        setToken(sessionStorage.getItem('isUserLogged'));
    }, []);

    useEffect(() => {
    }, [token]);
    return (
        <>
            <Fragment>
                <Toolbar
                    sx={{
                        background: '#2f9a27',
                        borderBottom: 1,
                        borderColor: 'divider',
                        display: "flex",
                        flexWrap: "wrap"
                    }}
                >
                    <Container
                        style={{
                            display: 'flex',
                            flex: 1,
                            flexDirection: "row",
                            flexWrap: 'wrap'
                        }}
                    >
                        <Link to='homepage' style={{ textDecoration: 'none' }}>
                            <Box sx={{ display: "flex" }}>

                                <GiCook size={40} color="white" />
                                <Typography
                                    align='left'
                                    color='#FFFFFF'
                                    component='h2'
                                    variant='h4'
                                    noWrap
                                >
                                    {"The healthy Chef"}
                                </Typography>
                            </Box>

                        </Link>
                        <Box></Box>
                    </Container>

                    {/* ***********************************************************/}
                    <Box sx={{ flexGrow: 1, display: { xs: 'flex', md: 'none', justifyContent:"right" } }}>
                        <IconButton
                            size="large"
                            aria-label="account of current user"
                            aria-controls="menu-appbar"
                            aria-haspopup="true"
                            onClick={handleOpenNavMenu}
                            color="inherit"
                        >
                            <AiOutlineMenu color={'#92279A'} />
                        </IconButton>
                        <Menu
                            id="menu-appbar"
                            anchorEl={anchorElNav}
                            anchorOrigin={{
                                vertical: 'bottom',
                                horizontal: 'left',
                            }}
                            keepMounted
                            transformOrigin={{
                                vertical: 'top',
                                horizontal: 'left',
                            }}
                            open={Boolean(anchorElNav)}
                            onClose={handleCloseNavMenu}
                            sx={{
                                display: { xs: 'block', md: 'none',  },
                            }}
                        >
                            <Box sx={{display:"flex", flexDirection:"column"}}>
                            <CookingRecepieDropdownmenu />
                            <Link to='blogs' style={{ textDecoration: 'none' }} onClick={handleCloseNavMenu}>
                                <HeaderButton buttonName='Blogovi' />
                            </Link>

                            {isUserLoggedIn !== true ?
                                <Link to='/Login' style={{ textDecoration: 'none' }} onClick={handleCloseNavMenu}>
                                    <HeaderButton buttonName='Prijavi se' />
                                </Link> : ""}

                            {isUserLoggedIn !== true ?
                                <Link to='/Register' style={{ textDecoration: 'none' }} onClick={handleCloseNavMenu}>
                                    <HeaderButton buttonName='Registruj se' />
                                </Link> : ""}



                            {isUserLoggedIn == true && isUserAuthor == true ?
                                <Link to='CreateBlogPost' style={{ textDecoration: 'none' }} onClick={handleCloseNavMenu}>
                                    <HeaderButton buttonName='Kreiraj blog' />
                                </Link> : ""}

                            {isUserLoggedIn == isUserAuthor && isUserAuthor == true ?
                                <Link to='CreateCookingRecepie' style={{ textDecoration: 'none' }} onClick={handleCloseNavMenu}>
                                    <HeaderButton buttonName='Kreiraj recept' />
                                </Link> : ""}

                            {isUserLoggedIn ?
                                <Link to='/homepage' onClick={handleCloseNavMenu}>
                                    <Button onClick={handleLogout} >
                                        <LogoutIcon style={{ color: '#92279A' }} />
                                    </Button>
                                </Link> : ""}
                                </Box>
                        </Menu>
                    </Box>


                    {/* ***********************************************************/}
                    <Box sx={{ flexGrow: 1, display: { xs: 'none', md: 'flex', justifyContent: "right" } }}>
                        <CookingRecepieDropdownmenu />
                        <Link to='blogs' style={{ textDecoration: 'none' }}>
                            <HeaderButton buttonName='Blogovi' />
                        </Link>

                        {isUserLoggedIn !== true ?
                            <Link to='/Login' style={{ textDecoration: 'none' }}>
                                <HeaderButton buttonName='Prijavi se' />
                            </Link> : ""}

                        {isUserLoggedIn !== true ?
                            <Link to='/Register' style={{ textDecoration: 'none' }}>
                                <HeaderButton buttonName='Registruj se' />
                            </Link> : ""}



                        {isUserLoggedIn == true && isUserAuthor == true ?
                            <Link to='CreateBlogPost' style={{ textDecoration: 'none' }}>
                                <HeaderButton buttonName='Kreiraj blog' />
                            </Link> : ""}

                        {isUserLoggedIn == isUserAuthor && isUserAuthor == true ?
                            <Link to='CreateCookingRecepie' style={{ textDecoration: 'none' }}>
                                <HeaderButton buttonName='Kreiraj recept' />
                            </Link> : ""}

                        {isUserLoggedIn ?
                            <Link to='/homepage'>
                                <Button onClick={handleLogout}>
                                    <LogoutIcon style={{ color: '#92279A' }} />
                                </Button>
                            </Link> : ""}

                    </Box>
                    {isUserLoggedIn === true && sessionStorage.getItem('typeOfUser') === "Author" ?
                        <Link to='/authorProfile' state={{ username: sessionStorage.getItem('username') }}>
                            <Button>
                                <Avatar alt='userPicture'
                                    sx={{ height: 48, width: 48 }}
                                    src ={sessionStorage.getItem('profilePicture') !== null ? 
                                            `data:image/jpeg;base64,${sessionStorage.getItem('profilePicture')}` : "" } />
                            </Button>
                        </Link>
                        : ""}

                    {isUserLoggedIn === true && sessionStorage.getItem('typeOfUser') === "Reader" ?
                        <Link to='/readerProfile' state={{ username: sessionStorage.getItem('username')}} style={{ textDecoration: 'none' }}>
                            <Button>
                                <Avatar alt='userPicture'
                                    sx={{ height: 48, width: 48, }}
                                    src={sessionStorage.getItem('profilePicture') !== null ? 
                                            `data:image/jpeg;base64,${sessionStorage.getItem('profilePicture')}` : "" }
                                     />
                                    
                            </Button> </Link> : ""}
                </Toolbar>
                <Outlet />
            </Fragment>
        </>
    );
}
export default Header;

