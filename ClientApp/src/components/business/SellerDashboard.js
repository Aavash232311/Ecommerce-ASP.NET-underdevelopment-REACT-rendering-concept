import React, { Component } from "react";
import "../../style/seller.css";

export class SellerDashboard extends Component {
    render() {
        return (
            <div>
               <div id="sideNav"> 
                 <span id="logo" onClick={() => {window.location.href = "/seller_control_panel"}}>
                    Management
                 </span>
                 <ul id="list">
                    <li className="elem" onClick={() => {window.location.href = "/product_seller_form_Add"}}>
                        Add Product
                    </li>
                    <li className="elem">
                        Shop Page
                    </li>
                    <li className="elem">
                        Order Page
                    </li>
                    <li className="elem" onClick={() => {window.location.href = "/product_manager"}}>
                        Manage Product
                    </li>
                 </ul>
               </div>
            </div>
        )
    }
}