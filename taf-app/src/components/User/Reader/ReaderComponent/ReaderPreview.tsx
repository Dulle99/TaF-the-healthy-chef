import { Container } from "@mui/material";
import { useEffect } from "react";
import { useLocation } from "react-router-dom";
import IReaderUsername from "../ReaderInterface/IReaderUsername";
import ReaderInformation from "./ReaderInformation";

function ReaderPreview() {
    const location = useLocation().state as IReaderUsername;
    useEffect(()=>{
        document.title = "Pregled profila";
    },[]);
    return (<>
        <Container>
            <ReaderInformation username={location.username} />
        </Container>
    </>);
}
export default ReaderPreview;