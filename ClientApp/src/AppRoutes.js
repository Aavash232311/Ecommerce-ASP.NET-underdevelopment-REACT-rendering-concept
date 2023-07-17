import { Home } from "./components/Home";
import { Register } from "./components/Register";
import { Login } from "./components/Login";
import { Admin } from "./components/admin";
import { RegisterSeller } from "./components/business/RegisterSeller";
import { Seller } from "./components/business/seller";
import { AddProduct } from "./components/business/AddProduct";
import { ProductManager } from "./components/business/ManageProduct";
import { AdminCategories } from "./components/Admin/AdminCat";

const AppRoutes = [
  {
    index: true,
    element: <Home />,
    login: false,
    roles: ["Client"]
  },
  {
    path: "/register",
    element: <Register />,
    login: false,
    roles: ["Client"]
  },

  {
    path: "/login",
    element: <Login />,
    login: false,
    roles: ["Client"]
  },

  {
    path: "/admin",
    element: <Admin />,
    login: true,
    roles: ["Admin"]
  },
  {
    path: '/registerResturent',
    element: <RegisterSeller />,
    login: true,
    roles: ["Client", "Admin"]
  },
  {
    path: "/seller_control_panel",
    element: <Seller />,
    login: true,
    roles: ["Seller"]
  },
  {
    path: "product_seller_form_Add",
    element: <AddProduct />,
    login: true,
    roles: ["Seller"]
  },
  {
    path: "product_manager",
    element: <ProductManager reverse_engineer={false} />,
    login: true,
    roles: ["Seller"]
  },
  {
    path: "admin_AdminCategories",
    element: <AdminCategories />,
    login: true,
    roles: ['Admin']

  }
];

export default AppRoutes;
