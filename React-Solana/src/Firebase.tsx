// Import the functions you need from the SDKs you need
import {initializeApp} from 'firebase/app';
import "firebase/analytics";
import "firebase/auth";
import {getFirestore, collection, getDocs} from 'firebase/firestore/lite';
import {getDatabase, ref, set, child, get} from "firebase/database";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
    apiKey: "AIzaSyAZjuhDPmon1DnzRutD9a9YrlDxllu0mQQ",
    authDomain: "solana-30d78.firebaseapp.com",
    databaseURL: "https://solana-30d78-default-rtdb.firebaseio.com",
    projectId: "solana-30d78",
    storageBucket: "solana-30d78.appspot.com",
    messagingSenderId: "516735150719",
    appId: "1:516735150719:web:3ac592ff811e4d21022d4a",
    measurementId: "G-2FQW03CHT6"
};

const app = initializeApp(firebaseConfig);
const db = getDatabase(app);
const dbRef = ref(db);

export async function SendData(UserName: string, Password: string, Address: string) {
    console.log("Sending");
    await set(ref(db, 'users/' + UserName), {
        username: UserName,
        Password: Password,
        Address: Address,
    }).then(() => {
        console.log("Sending Completed");
    }).catch(error => {
        console.log("error-->" + error);
    });

}

export async function CheckData(UserName: string) {
    var value = "";
    console.log("Checking");
    await get(child(dbRef, `users/${UserName}`)).then((snapshot) => {
        if (snapshot.exists()) {
            console.log(snapshot.val());
            value = "Data available";
            return value;
        } else {
            value = "No data available";
            console.log("No data available");
            return value;
        }
    }).catch((error) => {
        value = "error";
        return value;
        console.error(error);
    });
    return value;
}