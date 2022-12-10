import { useLocation } from "react-router-dom";
import ICookingRecepieUpdate from "../CookingRecepieInterface/ICookingRecepieUpdate";
import CookingRecepieForm from "./CookingRecepieForm";

function CookingRecepieUpdateForm() {
    const location = useLocation().state as ICookingRecepieUpdate;
    return (<>
        <CookingRecepieForm cookingRecepieId={location.cookingRecepieId} cookingRecepieUpdate={location.cookingRecepieUpdate}  />
    </>);
}
export default CookingRecepieUpdateForm;