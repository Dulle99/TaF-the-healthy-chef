import React, { useEffect, useState } from 'react';
import {  Routes, Route, Link, useNavigate } from 'react-router-dom';
import Header from './components/Header/Header';
import Login from './components/Login/Login';
import Register from './components/Register/Register';
import Homepage from './components/Homepage/Homepage';
import CookingRecepiesPreview from './components/CookingRecepie/CookingRecepieComponents/CookingRecepiesPreviews';
import BlogPreviews from './components/Blog/BlogComponents/BlogPreviews';
import CookingRecepieForm from './components/CookingRecepie/CookingRecepieComponents/CookingRecepieForm';
import BlogForm from './components/Blog/BlogComponents/BlogForm';
import CookingRecepie from './components/CookingRecepie/CookingRecepieComponents/CookingRecipe';
import Blog from './components/Blog/BlogComponents/Blog';
import AuthorPreview from './components/User/Author/AuthorComponents/AuthorPreview';
import AuthorProfile from './components/User/Author/AuthorComponents/AuthorProfile';
import ReaderProfile from './components/User/Reader/ReaderComponent/ReaderProfile';
import CookingRecepieUpdateForm from './components/CookingRecepie/CookingRecepieComponents/CookingRecpieUpdateForm';
import BlogUpdateForm from './components/Blog/BlogComponents/BlogUpdateForm';
import AuthorUpdateForm from './components/User/Author/AuthorComponents/AuthorUpdateForm';
import ReaderUpdateForm from './components/User/Reader/ReaderComponent/ReaderUpdateForm';
import ReaderPreview from './components/User/Reader/ReaderComponent/ReaderPreview';


function App() {
  let navigate = useNavigate();
  const [token, setToken] = useState<string | null>(null);

  function userLogged(){
    setToken(sessionStorage.getItem('token'));
  }

  useEffect(()=>{
    if(window.location.pathname=== '/'){
      navigate("/homepage", { replace: true });
    }
  },[]);

  useEffect(() => {
    setToken(sessionStorage.getItem('token'));
  }, [token]);

  if (token === null) {
    return (
      <>
          <Routes>
            <Route path='/' element={<Header userLogged={userLogged} />}>
              <Route path='homepage' element={<Homepage />} />
              <Route path='breakfastCookingRecepies' element={<CookingRecepiesPreview typeOfMeal='Dorucak' />} />
              <Route path='lunchCookingRecepies' element={<CookingRecepiesPreview typeOfMeal='Rucak' />} />
              <Route path='dinerCookingRecepies' element={<CookingRecepiesPreview typeOfMeal='Vecera' />} />
              <Route path='snacksCookingRecepies' element={<CookingRecepiesPreview typeOfMeal='Uzina' />} />
              <Route path='blogs' element={<BlogPreviews />} />
              <Route path='cookingRecepie' element={<CookingRecepie />} />
              <Route path='blog' element={<Blog />} />
            </Route>
            <Route path="Login" element={<Login userLogged={userLogged} />} />
            <Route path="Register" element={<Register userLogged={userLogged} />} />
          </Routes>
        
      </>
    );
  }
  else {
    return (
      <>
      {sessionStorage.getItem('typeOfUser') === "Author" ? 
          <Routes>
            <Route path='/' element={<Header userLogged={userLogged} />}>
              <Route path='homepage' element={<Homepage />} />
              <Route path='breakfastCookingRecepies' element={<CookingRecepiesPreview typeOfMeal='Dorucak' />} />
              <Route path='lunchCookingRecepies' element={<CookingRecepiesPreview typeOfMeal='Rucak' />} />
              <Route path='dinerCookingRecepies' element={<CookingRecepiesPreview typeOfMeal='Vecera' />} />
              <Route path='snacksCookingRecepies' element={<CookingRecepiesPreview typeOfMeal='Uzina' />} />
              <Route path='blogs' element={<BlogPreviews />} />
              <Route path='CreateCookingRecepie' element={<CookingRecepieForm />} />
              <Route path='CreateBlogPost' element={<BlogForm />} />
              <Route path='cookingRecepie' element={<CookingRecepie />} />
              <Route path='blog' element={<Blog />} />
              <Route path='author' element={<AuthorPreview />} />
              <Route path='reader' element={<ReaderPreview />}/>
              <Route path='authorProfile' element={<AuthorProfile />} />
              <Route path='updateCookingRecepie' element={<CookingRecepieUpdateForm />} />
              <Route path='updateBlog' element={<BlogUpdateForm />} />
              <Route path='updateAuthor' element = {<AuthorUpdateForm />}/>
            </Route>
          </Routes>
        : 
          <Routes>
            <Route path='/' element={<Header userLogged={userLogged}/>}>
              <Route path='homepage' element={<Homepage />} />
              <Route path='breakfastCookingRecepies' element={<CookingRecepiesPreview typeOfMeal='Dorucak' />} />
              <Route path='lunchCookingRecepies' element={<CookingRecepiesPreview typeOfMeal='Rucak' />} />
              <Route path='dinerCookingRecepies' element={<CookingRecepiesPreview typeOfMeal='Vecera' />} />
              <Route path='snacksCookingRecepies' element={<CookingRecepiesPreview typeOfMeal='Uzina' />} />
              <Route path='blogs' element={<BlogPreviews />} />
              <Route path='cookingRecepie' element={<CookingRecepie />} />
              <Route path='blog' element={<Blog />} />
              <Route path='author' element={<AuthorPreview />} />
              <Route path='reader' element={<ReaderPreview />}/>
              <Route path='readerProfile' element={<ReaderProfile />} />
              <Route path='updateReader' element = {<ReaderUpdateForm />} />
            </Route>
          </Routes>
         }
      </>
    );
  }
}

export default App;
