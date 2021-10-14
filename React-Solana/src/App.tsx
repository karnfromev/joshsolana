import './App.css';
import {Wallet} from './Wallet';
import "./Firebase";
import {useWallet} from "@solana/wallet-adapter-react";
import {SendData, CheckData} from './Firebase';
import Signup from "./Signup";


function App() {

    return (
        <div className="App">
            <div className="content">
                <div>
                 <Signup/>
                </div>
            </div>
        </div>
    );


}

export default App;
