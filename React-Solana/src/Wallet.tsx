import React, {FC, useMemo, useState} from 'react';
import {ConnectionProvider, WalletProvider} from '@solana/wallet-adapter-react';
import {WalletAdapterNetwork} from '@solana/wallet-adapter-base';
import {
    getLedgerWallet,
    getPhantomWallet,
    getSlopeWallet,
    getSolflareWallet,
    getSolletExtensionWallet,
    getSolletWallet,
    getTorusWallet,
} from '@solana/wallet-adapter-wallets';
import {
    WalletModalProvider,
    WalletDisconnectButton,
    WalletMultiButton,
    WalletConnectButton,
    WalletModalContext
} from '@solana/wallet-adapter-react-ui';


import {clusterApiUrl} from '@solana/web3.js';
import {WalletMulti} from "./WalletMulti";

// Default styles that can be overridden by your app
require('@solana/wallet-adapter-react-ui/styles.css');

export const Wallet: FC = () => {
    // Can be set to 'devnet', 'testnet', or 'mainnet-beta'
    const network = WalletAdapterNetwork.Devnet;

    // You can also provide a custom RPC endpoint
    const endpoint = useMemo(() => clusterApiUrl(network), [network]);
    // @solana/wallet-adapter-wallets includes all the adapters but supports tree shaking --
    // Only the wallets you configure here will be compiled into your application
    const wallets = useMemo(() => [
        getPhantomWallet(),
        getSlopeWallet(),
        getSolflareWallet(),
        getTorusWallet({
            options: {clientId: 'Get a client ID @ https://developer.tor.us'}
        }),
        getLedgerWallet(),
        getSolletWallet({network}),
        getSolletExtensionWallet({network}),
    ], [network]);
    return (
        <ConnectionProvider endpoint={endpoint}>
            <WalletProvider wallets={wallets}>
                <WalletModalProvider>
                    <div>
                        <WalletMultiButton style={
                            {
                                marginBottom : "35px"
                            }
                        }

                            onClick={ // set connect true when Connect to wallet button is pressed
                                () => {
                                    console.log("Connected button is pressed")
                                    localStorage.setItem("connected", "true")
                                }

                            }
                        />
                        <WalletDisconnectButton
                            onClick={ // set conect false when disconnect is pressed
                                (event) => {
                                    localStorage.setItem("connected", "false")
                                    console.log("Disconnect button is pressed")
                                }
                            }/>
                        <WalletMulti/>
                    </div>
                </WalletModalProvider>
            </WalletProvider>
        </ConnectionProvider>
    );
};