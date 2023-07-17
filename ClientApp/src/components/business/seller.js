import React, { Component } from "react";
import { SellerDashboard } from "./SellerDashboard";


export class Seller extends Component {
    render() {
        return (
            <div>
                <SellerDashboard />
                <center>
                    <h5>Your Trade Center</h5>
                </center>
            </div>
        )
    }
}