import { useLocation } from "react-router-dom";
import ICookingRecepieUpdate from "../../CookingRecepie/CookingRecepieInterface/ICookingRecepieUpdate";
import IBlogUpdate from "../BlogInterfaces/IBlogUpdate";
import BlogForm from "./BlogForm";

function BlogUpdateForm(){
    const location = useLocation().state as IBlogUpdate;
    return(<>
        <BlogForm blogId={location.blogId} blogUpdate={location.blogUpdate}/>
    </>)
}
export default BlogUpdateForm;