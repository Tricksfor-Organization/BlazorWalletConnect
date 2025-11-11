# Cloudflare Pages Deployment Guide

This guide explains how to set up and deploy the BlazorWalletConnect demo application to Cloudflare Pages using GitHub Actions.

## Overview

The deployment workflow automatically builds and publishes the Blazor WebAssembly demo application to Cloudflare Pages whenever changes are pushed to the `main` branch or when changes are made to the demo or library source code.

## Prerequisites

1. **Cloudflare Account** - Sign up at [cloudflare.com](https://www.cloudflare.com/)
2. **GitHub Repository** - This repository with admin access to configure secrets
3. **Cloudflare API Token** - With Pages permissions
4. **Cloudflare Account ID** - Found in your Cloudflare dashboard

## Setup Instructions

### Step 1: Create Cloudflare Pages Project

**Option A: Automatic Creation (Recommended)**

The workflow will automatically create the project on first deployment if it doesn't exist.

**Option B: Manual Creation**

1. Log in to your [Cloudflare Dashboard](https://dash.cloudflare.com/)
2. Navigate to **Pages** in the left sidebar
3. Click **Create a project**
4. Select **Direct Upload**
5. Enter project name: `blazorwalletconnect-demo`
6. Click **Create project**

**Note:** The project name must match `blazorwalletconnect-demo` in the workflow file, or update the workflow to use your custom name.

### Step 2: Get Cloudflare Account ID

1. In your Cloudflare Dashboard, go to **Pages**
2. Your Account ID is displayed in the URL or on the right side of the dashboard
3. Copy this ID - it looks like: `a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6`

### Step 3: Create Cloudflare API Token

1. Go to [Cloudflare API Tokens](https://dash.cloudflare.com/profile/api-tokens)
2. Click **Create Token**
3. Use the **Edit Cloudflare Workers** template or create a custom token
4. Configure the token with these permissions:
   - **Account** → **Cloudflare Pages** → **Edit**
5. Set Account Resources to include your account
6. Click **Continue to summary** and then **Create Token**
7. **Important:** Copy the token immediately - it won't be shown again!

### Step 4: Configure GitHub Secrets

1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Add the following secrets:

   **Secret 1:**
   - Name: `CLOUDFLARE_API_TOKEN`
   - Value: Your Cloudflare API token from Step 3

   **Secret 2:**
   - Name: `CLOUDFLARE_ACCOUNT_ID`
   - Value: Your Cloudflare Account ID from Step 2

### Step 5: Configure Project Name (Optional)

By default, the project name is `blazorwalletconnect-demo`. To change it:

1. Edit `.github/workflows/deploy-demo-cloudflare.yml`
2. Find the line with `projectName: blazorwalletconnect-demo`
3. Change to your desired project name
4. Ensure the project exists in Cloudflare or will be created automatically

## Deployment

### Automatic Deployment

The workflow automatically deploys when:
- A new release is published or created on GitHub
- You can also trigger it manually via workflow_dispatch

### Creating a Release

To deploy via release:

1. Go to your repository on GitHub
2. Click on **Releases** (right sidebar)
3. Click **Draft a new release**
4. Create a new tag (e.g., `v1.0.0`)
5. Fill in release title and description
6. Click **Publish release**
7. The deployment workflow will trigger automatically

### Manual Deployment

You can also trigger deployment manually:

1. Go to **Actions** tab in GitHub
2. Select **Deploy Demo to Cloudflare Pages** workflow
3. Click **Run workflow**
4. Select the branch and environment
5. Click **Run workflow**

## URLs

After deployment, your application will be available at:

- **Production URL:** `https://blazorwalletconnect-demo.pages.dev`
- **Preview URLs:** `https://<branch-name>.blazorwalletconnect-demo.pages.dev`

Replace `blazorwalletconnect-demo` with your actual project name if you changed it.

## Workflow Features

### Build Process

1. **Restores NuGet and npm packages** with caching for faster builds
2. **Builds the library project** first to ensure all dependencies are ready
3. **Publishes the demo application** in Release configuration
4. **Optimizes for production** with proper MIME types and caching headers

### Cloudflare Configuration

The workflow automatically creates two configuration files:

#### `_headers` File
- Sets security headers (X-Frame-Options, X-Content-Type-Options)
- Configures proper MIME types for WebAssembly files
- Sets cache control headers for optimal performance
- Ensures service worker updates properly

#### `_redirects` File
- Enables SPA routing by redirecting all routes to `index.html`
- Ensures Blazor routing works correctly

### Deployment Environments

- **Production:** Deploys from `main` branch to production URL
- **Preview:** Deploys from other branches to preview URLs with branch name

## Troubleshooting

### Common Issues

1. **"Invalid API Token" Error**
   - Verify the token has correct permissions (Cloudflare Pages Edit)
   - Ensure the token hasn't expired
   - Check that the secret is correctly named `CLOUDFLARE_API_TOKEN`

2. **"Project not found" Error**
   - The workflow will attempt to create the project automatically
   - If it fails, manually create the project in Cloudflare Pages dashboard
   - Ensure project name matches `blazorwalletconnect-demo` in the workflow file
   - Or update the workflow with your custom project name

3. **"Account ID invalid" Error**
   - Double-check your Account ID from Cloudflare dashboard
   - Ensure the secret is correctly named `CLOUDFLARE_ACCOUNT_ID`

4. **WebAssembly Loading Issues**
   - Check browser console for MIME type errors
   - The `_headers` file should resolve these automatically
   - Verify files are published to `wwwroot` directory

5. **Routing Issues (404 on refresh)**
   - The `_redirects` file handles SPA routing
   - Ensure the file is created correctly
   - Check Cloudflare Pages dashboard for redirect rules

### Debug Mode

To see more details during deployment:

1. Go to **Actions** tab in GitHub
2. Click on the failed workflow run
3. Expand the steps to see detailed logs
4. Check the "Publish to Cloudflare Pages" step for deployment details

## Base Path Configuration

If you need to deploy to a subdirectory (not common for Cloudflare Pages):

1. Update `wwwroot/index.html` in the demo project:
   ```html
   <base href="/your-subdirectory/" />
   ```

2. Modify the workflow to handle the base path:
   ```bash
   sed -i 's|<base href="/" />|<base href="/your-subdirectory/" />|g' ./publish/wwwroot/index.html
   ```

## Custom Domain

To use a custom domain:

1. Go to your Cloudflare Pages project
2. Click **Custom domains**
3. Click **Set up a custom domain**
4. Follow Cloudflare's instructions to:
   - Add DNS records
   - Verify domain ownership
   - Enable HTTPS (automatic)

## Performance Optimization

The workflow includes several optimizations:

- **Caching:** NuGet and npm packages are cached
- **Immutable Assets:** Static assets use long cache times
- **Compression:** Cloudflare automatically compresses responses
- **CDN:** Global CDN distribution for fast loading worldwide
- **HTTP/3:** Automatic HTTP/3 support

## Security Headers

The deployment includes security headers:

- `X-Frame-Options: SAMEORIGIN` - Prevents clickjacking
- `X-Content-Type-Options: nosniff` - Prevents MIME sniffing
- `Referrer-Policy: strict-origin-when-cross-origin` - Controls referrer info

## Monitoring

Monitor your deployments:

1. **GitHub Actions:** View build and deployment logs
2. **Cloudflare Dashboard:** View deployment history and analytics
3. **Cloudflare Analytics:** Track page views, performance, and errors

## Cost

Cloudflare Pages offers:
- **Free tier:** Unlimited sites, 500 builds/month, unlimited requests
- **Paid tier:** $20/month for concurrent builds and additional features

The free tier is usually sufficient for demo applications.

## Additional Resources

- [Cloudflare Pages Documentation](https://developers.cloudflare.com/pages/)
- [Blazor WebAssembly Hosting](https://docs.microsoft.com/aspnet/core/blazor/host-and-deploy/webassembly)
- [GitHub Actions Documentation](https://docs.github.com/actions)
- [Cloudflare Pages Action](https://github.com/cloudflare/pages-action)

## Support

If you encounter issues:

1. Check the [GitHub Actions logs](../../actions)
2. Review the [Cloudflare Pages documentation](https://developers.cloudflare.com/pages/)
3. Open an issue in this repository
4. Check Cloudflare community forums

## Workflow File Location

The workflow file is located at:
```
.github/workflows/deploy-demo-cloudflare.yml
```

You can customize the workflow to fit your specific needs by editing this file.
