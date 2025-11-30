import { useLocation } from "react-router-dom";
import Main from "./Partials/Main";
import Header from "./Partials/Header";
import Footer from "./Partials/Footer";

export default function App() {

  useLocation();
  window.scrollTo({
    top: 0,
    left: 0,
    behavior: 'instant'
  });
  
  return (
    <>
      <Header />
      <Main />
      <Footer />
    </>
  );
}