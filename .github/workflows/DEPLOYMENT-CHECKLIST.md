# 📋 Cloudflare Pages Deployment Checklist

Use this checklist to ensure your Cloudflare Pages deployment is set up correctly.

## Pre-Deployment Setup

### Cloudflare Account
- [ ] Created Cloudflare account at https://cloudflare.com
- [ ] Verified email address
- [ ] Logged into Cloudflare dashboard

### API Token Creation
- [ ] Navigated to https://dash.cloudflare.com/profile/api-tokens
- [ ] Clicked "Create Token"
- [ ] Selected "Edit Cloudflare Workers" template
- [ ] Added permission: Account → Cloudflare Pages → Edit
- [ ] Copied API token (save it securely!)

### Account ID
- [ ] Found Account ID in Cloudflare dashboard
- [ ] Copied Account ID (format: a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6)

### GitHub Secrets Configuration
- [ ] Opened GitHub repository
- [ ] Navigated to Settings → Secrets and variables → Actions
- [ ] Added secret: `CLOUDFLARE_API_TOKEN` with your API token
- [ ] Added secret: `CLOUDFLARE_ACCOUNT_ID` with your Account ID
- [ ] Added secret: `WALLETCONNECT_PROJECT_ID` with your WalletConnect Project ID
- [ ] Verified all three secrets are saved

### Repository Files
- [ ] Workflow file exists: `.github/workflows/deploy-demo-cloudflare.yml`
- [ ] Documentation exists: `.github/workflows/README-CLOUDFLARE.md`
- [ ] Quick start exists: `.github/workflows/QUICKSTART-CLOUDFLARE.md`
- [ ] All files committed to repository
- [ ] Files pushed to GitHub

## First Deployment

### Create Release
- [ ] Decided on version number (e.g., v1.0.0)
- [ ] Navigated to GitHub → Releases
- [ ] Clicked "Draft a new release"
- [ ] Created new tag with version number
- [ ] Added release title
- [ ] Added release notes/description
- [ ] Clicked "Publish release"

### Trigger Deployment
- [ ] Release published successfully
- [ ] Workflow started automatically in GitHub Actions
- [ ] OR manually triggered workflow from Actions tab

### Verify Build
- [ ] Workflow started in GitHub Actions
- [ ] Setup steps completed successfully
- [ ] Build steps completed successfully
- [ ] Publish step completed successfully
- [ ] Cloudflare deployment step completed successfully

### Check Deployment
- [ ] Visited https://blazorwalletconnect-demo.pages.dev
- [ ] Demo site loads correctly
- [ ] No console errors in browser
- [ ] Wallet connect button appears
- [ ] Can interact with the demo

## Post-Deployment Verification

### Functionality Tests
- [ ] Page loads without errors
- [ ] All assets load (CSS, JS, images)
- [ ] Service worker registers successfully
- [ ] Blazor WebAssembly initializes
- [ ] WalletConnect modal appears on button click
- [ ] Can connect wallet (if you have one)

### Performance Checks
- [ ] Page loads quickly (< 3 seconds)
- [ ] No 404 errors in network tab
- [ ] WASM files load with correct MIME type
- [ ] Static assets are cached properly

### Routing Tests
- [ ] Home page loads at root URL
- [ ] Can navigate to different pages (if multiple)
- [ ] Refresh works on sub-routes (no 404)
- [ ] Browser back/forward buttons work

### Security Headers
- [ ] Check response headers in browser DevTools
- [ ] Verify `X-Frame-Options` header present
- [ ] Verify `X-Content-Type-Options` header present
- [ ] Verify HTTPS is enabled

## Cloudflare Dashboard

### Project Setup
- [ ] Project appears in Cloudflare Pages dashboard
- [ ] Project name is correct: `blazorwalletconnect-demo`
- [ ] Latest deployment shows as "Active"
- [ ] No error messages in deployment logs

### Configuration
- [ ] Build settings are correct (if manually configured)
- [ ] Custom domain configured (optional)
- [ ] Analytics enabled (optional)

## Documentation

### Updated Files
- [ ] Main README.md includes live demo link
- [ ] Demo README.md includes live demo link
- [ ] Deployment documentation is accessible
- [ ] All links work correctly

### Knowledge Base
- [ ] Team knows where to find deployment docs
- [ ] Team knows how to trigger manual deployment
- [ ] Team knows how to check deployment status
- [ ] Team knows where to find troubleshooting guide

## Optional Enhancements

### Custom Domain
- [ ] Custom domain purchased/available
- [ ] DNS records configured in Cloudflare
- [ ] Custom domain added to Pages project
- [ ] SSL certificate active
- [ ] Domain resolves to demo site

### Analytics
- [ ] Cloudflare Web Analytics enabled
- [ ] Google Analytics added (if needed)
- [ ] Custom analytics configured (if needed)

### Performance
- [ ] Images optimized
- [ ] Unnecessary files removed
- [ ] Service worker configured optimally
- [ ] Cache policies reviewed

## Ongoing Maintenance

### Regular Checks
- [ ] Monitor deployment success/failure
- [ ] Check for security updates
- [ ] Review analytics periodically
- [ ] Test functionality after updates

### Updates
- [ ] Keep dependencies updated
- [ ] Review Cloudflare features
- [ ] Optimize based on analytics
- [ ] Update documentation as needed

## Troubleshooting Reference

### Common Issues
- [ ] Know how to check GitHub Actions logs
- [ ] Know how to check Cloudflare deployment logs
- [ ] Know how to regenerate API token
- [ ] Know how to update secrets
- [ ] Know where troubleshooting docs are located

### Emergency Contacts
- [ ] GitHub repository admin contact
- [ ] Cloudflare account admin contact
- [ ] Development team lead contact

## Success Criteria

✅ **Deployment Successful When:**
- Demo site is accessible at production URL
- All functionality works as expected
- No console errors or warnings
- Page loads within acceptable time
- Wallet connection works properly
- Automatic deployments work on push

## Notes

**Production URL:** https://blazorwalletconnect-demo.pages.dev

**Workflow File:** `.github/workflows/deploy-demo-cloudflare.yml`

**Documentation:**
- Quick Start: `.github/workflows/QUICKSTART-CLOUDFLARE.md`
- Full Guide: `.github/workflows/README-CLOUDFLARE.md`
- Summary: `.github/workflows/DEPLOYMENT-SUMMARY.md`

**Cloudflare Dashboard:** https://dash.cloudflare.com/

**GitHub Actions:** https://github.com/Tricksfor-Organization/BlazorWalletConnect/actions

---

## Sign Off

- [ ] All items checked
- [ ] Deployment verified working
- [ ] Team notified
- [ ] Documentation complete

**Deployed by:** _______________

**Date:** _______________

**Demo URL verified:** ✅ / ❌

**Notes:** _____________________________
