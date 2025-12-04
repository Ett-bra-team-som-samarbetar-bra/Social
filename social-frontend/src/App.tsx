import { useLocation } from "react-router-dom";
import Main from "./Partials/Main";
import Footer from "./Partials/Footer";
import Header from "./Partials/Header";

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