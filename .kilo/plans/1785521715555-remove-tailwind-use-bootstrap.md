# Plan: Remove Tailwind CSS and Replace with Bootstrap 5

## Context

The Blazor WebAssembly project (`ProcessoSelecao.Blazor`) currently uses Tailwind CSS loaded via CDN (`<script src="https://cdn.tailwindcss.com"></script>` in `App.razor`). Bootstrap 5.3.3 is already present in `wwwroot/lib/bootstrap/dist/` but is not referenced. The goal is to remove Tailwind and use the existing Bootstrap installation.

## Current State

- **Tailwind**: Loaded via CDN in `App.razor` line 9
- **Bootstrap 5.3.3**: Already in `wwwroot/lib/bootstrap/dist/` (CSS + JS)
- **Tailwind classes used**: ~596 `class=` attributes across razor components and layouts
- **Custom CSS**: `wwwroot/css/formulario.css` and `wwwroot/app.css` are framework-independent
- **`Error.razor`** already uses Bootstrap class `text-danger`

## Changes Required

### 1. Update `App.razor` — Remove Tailwind, Add Bootstrap References
- **File**: `src/frontend/ProcessoSelecao.Blazor/Components/App.razor`
- Remove line 9: `<script src="https://cdn.tailwindcss.com"></script>`
- Add Bootstrap CSS: `<link rel="stylesheet" href="lib/bootstrap/dist/css/bootstrap.min.css" />`
- Add Bootstrap JS: `<script src="lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>`
- Keep the Material Icons link and existing CSS references

### 2. Replace Tailwind Classes with Bootstrap Equivalents

All razor components and layouts need Tailwind utility classes replaced with Bootstrap equivalents. The mapping is not 1:1 — some Tailwind utilities have no direct Bootstrap counterpart and will need custom CSS or Bootstrap's utility classes.

#### Key Class Mappings

| Tailwind | Bootstrap Equivalent |
|---|---|
| `bg-gray-50` | `bg-light` or custom `.bg-gray-50` |
| `bg-gray-800` | `bg-dark` or custom `.bg-gray-800` |
| `bg-gray-900` | `bg-black` or custom `.bg-gray-900` |
| `bg-blue-600` | `bg-primary` (Bootstrap default is `#0d6efd`, close to `#2563eb`) |
| `bg-red-500/600` | `bg-danger` |
| `bg-green-500/600` | `bg-success` |
| `bg-yellow-500` | `bg-warning` |
| `bg-cyan-500/600` | No direct equivalent — needs custom class |
| `bg-indigo-500/600` | No direct equivalent — needs custom class |
| `bg-white` | `bg-white` (same) |
| `bg-gray-50` | `bg-light` |
| `bg-gray-100` | `bg-light` |
| `bg-gray-200` | `bg-secondary` (or custom) |
| `text-gray-400/500/600/700/800/900` | `text-muted`, `text-secondary`, custom classes |
| `text-white` | `text-white` (same) |
| `text-blue-600` | `text-primary` |
| `text-red-500/600` | `text-danger` |
| `text-green-600` | `text-success` |
| `text-indigo-600` | Custom `.text-indigo` |
| `text-cyan-600` | Custom `.text-cyan` |
| `text-yellow-500/600` | `text-warning` |
| `text-xs/text-sm/text-base/text-lg/text-xl/text-2xl/text-3xl/text-4xl` | Bootstrap has `fs-1` through `fs-6` and `fs-sm` |
| `font-bold` | `fw-bold` |
| `font-semibold` | `fw-semibold` |
| `font-medium` | `fw-medium` |
| `font-light` | `fw-light` |
| `flex` | `d-flex` |
| `flex-col` | `flex-column` |
| `flex-row` | `flex-row` (same) |
| `flex-wrap` | `flex-wrap` (same) |
| `flex-1` | `flex-grow-1` |
| `flex-grow` | `flex-grow-1` |
| `items-center` | `align-items-center` |
| `justify-center` | `justify-content-center` |
| `justify-between` | `justify-content-between` |
| `justify-around` | `justify-content-around` |
| `gap-1/2/3/4/5` | `gap-1/2/3/4/5` (Bootstrap 5.3+ supports `gap-*`) |
| `px-2/3/4/5` | `px-2/3/4/5` (Bootstrap supports `px-*`) |
| `py-1/2/3/4` | `py-1/2/3/4` (Bootstrap supports `py-*`) |
| `p-2/3/4/5/6` | `p-2/3/4/5/6` (Bootstrap supports `p-*`) |
| `mx-auto` | `mx-auto` (same) |
| `mt-2/4/5/6` | `mt-2/4/5/6` (Bootstrap supports `mt-*`) |
| `mb-2/3/4/5/6/8` | `mb-2/3/4/5/6/8` (Bootstrap supports `mb-*`) |
| `ms-3` | `ms-3` (same) |
| `ps-3` | `ps-3` (same) |
| `border` | `border` (same) |
| `border-gray-200/300` | `border-secondary` or custom |
| `border-b` | `border-bottom` |
| `rounded` | `rounded` (same) |
| `rounded-lg` | `rounded-3` or custom |
| `rounded-full` | `rounded-pill` |
| `rounded-0.5rem` | `rounded` or custom |
| `shadow-md` | `shadow` |
| `shadow-sm` | `shadow-sm` |
| `w-full` | `w-100` |
| `w-64` | `w-25` (16rem) or custom |
| `w-11/12` | Custom class needed |
| `w-12 h-12` | `w-3 h-3` (3rem) or custom |
| `h-14` | Custom class needed |
| `h-full` | `h-100` |
| `min-h-screen` | Custom class needed |
| `min-h-[80vh]` | Custom class needed |
| `max-w-7xl` | `container` or custom |
| `max-w-6xl` | Custom class needed |
| `max-w-5xl` | Custom class needed |
| `max-w-2xl` | `container` or custom |
| `max-w-md` | Custom class needed |
| `max-w-[32rem]` | Custom class needed |
| `max-w-[90%]` | Custom style needed |
| `overflow-x-auto` | `overflow-x-auto` (same) |
| `overflow-y-auto` | `overflow-y-auto` (same) |
| `sticky top-0` | `sticky-top` |
| `sticky top-6` | Custom style needed |
| `fixed inset-0` | Custom style needed |
| `fixed top-0 left-0 right-0 bottom-0` | Custom style needed |
| `z-50` | `z-index: 50` (custom) |
| `list-none` | `list-unstyled` |
| `no-underline` | `text-decoration-none` |
| `hover:bg-gray-50` | `hover-bg-light` or custom |
| `hover:bg-gray-700` | Custom class needed |
| `hover:text-white` | `text-white` on hover (custom) |
| `hover:shadow-md` | `hover-shadow` or custom |
| `hover:underline` | `text-decoration-underline` |
| `hover:text-blue-700` | Custom class needed |
| `transition-colors` | Custom CSS transition |
| `animate-spin` | Bootstrap has no built-in spin — needs custom CSS or `@keyframes` |
| `border-l-3` | Custom class needed |
| `border-transparent` | `border-transparent` (same) |
| `border-l-transparent` | Custom class needed |
| `grid` | `row` (Bootstrap grid) or custom |
| `grid-cols-1/2/3/4` | `row-cols-1/2/3/4` or custom |
| `lg:grid-cols-5` | Custom responsive class needed |
| `md:grid-cols-2` | Custom responsive class needed |
| `col-span-2/3` | `col-md-6/col-lg-8` etc. |
| `lg:col-span-3` | `col-lg-8` |
| `space-y-4` | Custom CSS `.space-y-4` (Bootstrap has no gap utility for vertical spacing) |
| `text-danger` | `text-danger` (Bootstrap — already used in Error.razor) |
| `bg-gradient-to-br from-indigo-500 to-purple-600` | Custom class needed |
| `opacity-90` | `opacity-75` or custom |
| `cursor-pointer` | `cursor-pointer` (same) |
| `cursor-not-allowed` | `cursor-not-allowed` (same) |
| `appearance-none` | `appearance-none` (same) |
| `accent-color` | Custom style |
| `min-h-[80px]` | Custom class needed |
| `min-h-[600px]` | Custom class needed |
| `h-[600px]` | Custom class needed |
| `h-5/6` | Custom class needed |
| `w-[90%]` | Custom style needed |
| `max-h-[80vh]` | Custom class needed |
| `max-h-[20rem]` | Custom class needed |
| `text-sm` | `fs-6` or custom |
| `text-xs` | `fs-7` or custom |
| `text-base` | `fs-base` |
| `text-lg` | `fs-4` |
| `text-xl` | `fs-3` |
| `text-2xl` | `fs-2` |
| `text-3xl` | `fs-1` |
| `text-4xl` | Custom class needed |
| `text-5xl` | Custom class needed |
| `text-lg font-bold` | `fs-4 fw-bold` |
| `text-sm font-medium` | `fs-6 fw-medium` |
| `text-xs font-semibold` | `fs-7 fw-semibold` |
| `px-2.5 py-1 rounded text-xs` | `px-2 py-1 rounded fs-7` |
| `px-3 py-2 rounded text-sm` | `px-3 py-2 rounded fs-6` |
| `px-4 py-2 rounded` | `px-4 py-2 rounded` |
| `px-2.5 py-1 rounded text-xs hover:bg-blue-700` | Custom class needed |
| `bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition-colors` | `btn btn-primary` |
| `bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600` | `btn btn-secondary` |
| `bg-red-600 text-white px-2.5 py-1 rounded text-xs hover:bg-red-700` | `btn btn-danger btn-sm` |
| `bg-green-600 text-white px-3 py-1 rounded text-xs hover:bg-green-700` | `btn btn-success btn-sm` |
| `bg-cyan-600 text-white px-2.5 py-1 rounded text-xs hover:bg-cyan-700` | Custom class needed |
| `bg-yellow-500` | `bg-warning` |
| `bg-indigo-500/600` | Custom class needed |
| `bg-white rounded-lg shadow-md p-8` | `bg-white rounded shadow p-4` |
| `bg-white border border-gray-200 rounded-lg p-5` | `bg-white border rounded p-4` |
| `bg-white rounded-lg p-6` | `bg-white rounded p-4` |
| `bg-white rounded-lg w-full max-w-md p-6` | Custom class needed |
| `bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded` | `alert alert-danger` |
| `bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded` | `alert alert-success` |
| `bg-yellow-50 rounded-lg p-4` | Custom class needed |
| `bg-indigo-50 rounded-lg p-4` | Custom class needed |
| `bg-cyan-50 rounded-lg p-4` | Custom class needed |
| `bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded` | `alert alert-danger` |
| `bg-blue-50` | `bg-primary bg-opacity-10` or custom |
| `bg-gray-50 rounded-xl` | Custom class needed |
| `bg-gradient-to-br from-indigo-500 to-purple-600 text-white rounded-xl` | Custom class needed |
| `text-center py-10` | `text-center py-5` (Bootstrap uses different spacing scale) |
| `text-center py-10 bg-white border border-gray-200 rounded-lg` | Custom class needed |
| `text-center py-10 px-5 bg-gray-50 rounded-xl mt-10` | Custom class needed |
| `text-center py-10 text-gray-500` | Custom class needed |
| `dismiss` | Already defined in MainLayout.razor.css |
| `not-found` | Already defined in formulario.css |
| `carregando` | Already defined in formulario.css |
| `formulario-container` | Already defined in formulario.css |
| `progresso` / `barra-progresso` | Already defined in formulario.css |
| `indicadores-pagina` | Already defined in formulario.css |
| `conteudo-pagina` | Already defined in formulario.css |
| `botoes` | Already defined in formulario.css |
| `modal-overlay` / `modal-conteudo` | Already defined in formulario.css |
| `resumo-dados` | Already defined in formulario.css |
| `termo-checkbox` | Already defined in formulario.css |
| `modal-botoes` | Already defined in formulario.css |
| `btn-voltar` / `btn-confirmar` | Already defined in formulario.css |
| `numero-inscricao` | Already defined in formulario.css |
| `aviso` | Already defined in formulario.css |
| `erro` | Already defined in formulario.css |
| `campo-obrigatorio` | Already defined in formulario.css |
| `campo-checkbox` | Already defined in formulario.css |
| `info-cookies` | Already defined in formulario.css |
| `asterisco` | Already defined in formulario.css |
| `block text-sm font-medium mb-1` | `d-block fs-6 fw-medium mb-1` |
| `inline-block px-2 py-0.5 rounded text-xs font-semibold` | Custom class needed |
| `w-full px-3 py-2 border border-gray-300 rounded` | `w-100 px-3 py-2 border rounded form-control` |
| `w-full px-3 py-2 border border-gray-300 rounded focus:outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-200` | `form-control` (Bootstrap handles focus states) |
| `w-full px-3 py-2 border border-gray-300 rounded min-h-[80px]` | `form-control` with custom min-height |
| `w-full border-collapse` | `table table-bordered` (on parent) |
| `bg-gray-50` on table rows | `table-light` |
| `hover:bg-gray-50` on table rows | `table-hover` (on parent `<tbody>`) |
| `border-b border-gray-200` on table cells | `border-bottom` (Bootstrap table handles this) |
| `px-3 py-2.5` on table cells | Bootstrap table cell padding |
| `text-left font-semibold` on table headers | `text-start fw-semibold` |
| `select class="px-3 py-2 border border-gray-300 rounded text-sm"` | `form-select` |
| `input[type="checkbox"]` | `form-check-input` |
| `label` | `form-label` |
| `textarea` | `form-control` |
| `InputFile` | `form-control` |
| `bg-opacity-10` | Custom class needed |

### 3. Update `wwwroot/app.css`
- Remove comment "Tailwind handles everything"
- Keep the `.blazor-error-boundary` styles

### 4. Update `wwwroot/css/formulario.css`
- Some custom classes may need Bootstrap-specific overrides (e.g., `.form-control` focus styles already handled by Bootstrap)
- Keep framework-independent styles as-is

### 5. Remove Tailwind CDN Script
- Already covered in step 1

### 6. Verify Bootstrap JS is Loaded
- Ensure `bootstrap.bundle.min.js` is referenced in `App.razor` for Bootstrap components (tooltips, popovers, modals)

## Files to Modify

| File | Action |
|---|---|
| `Components/App.razor` | Remove Tailwind CDN, add Bootstrap CSS/JS refs |
| `Components/Layout/MainLayout.razor` | Replace Tailwind classes |
| `Components/Layout/NavMenu.razor` | Replace Tailwind classes |
| `Components/Layout/AdminLayout.razor` | Replace Tailwind classes |
| `Components/Layout/AvaliadorLayout.razor` | Replace Tailwind classes |
| `Components/Layout/PublicLayout.razor` | Replace Tailwind classes |
| `Components/Pages/Public/Home.razor` | Replace Tailwind classes |
| `Components/Pages/Public/ProcessoPublicList.razor` | Replace Tailwind classes |
| `Components/Pages/Admin/AdminIndex.razor` | Minimal (no Tailwind classes) |
| `Components/Pages/Admin/ProcessoList.razor` | Replace Tailwind classes |
| `Components/Pages/Admin/BaremaList.razor` | Replace Tailwind classes |
| `Components/Pages/Admin/CandidatoList.razor` | Replace Tailwind classes |
| `Components/Pages/Admin/DocumentoList.razor` | Replace Tailwind classes |
| `Components/Pages/Admin/AvaliadorList.razor` | Replace Tailwind classes |
| `Components/Pages/Admin/AvaliacaoList.razor` | Replace Tailwind classes |
| `Components/Pages/Avaliador/AvaliadorLogin.razor` | Replace Tailwind classes |
| `Components/Pages/Avaliador/AvaliadorPainel.razor` | Replace Tailwind classes |
| `Components/Pages/Avaliador/AvaliadorAvaliacao.razor` | Replace Tailwind classes |
| `Components/Pages/Formulario/Inscricao.razor` | Replace Tailwind classes (modal overlays) |
| `Components/Pages/Formulario/Pagina1.razor` | Minimal (uses custom CSS classes) |
| `Components/Pages/Formulario/Pagina2.razor` | Minimal (uses custom CSS classes) |
| `Components/Pages/Formulario/Pagina3.razor` | Minimal (uses custom CSS classes) |
| `Components/Pages/Formulario/Pagina4.razor` | Minimal (uses custom CSS classes) |
| `Components/Pages/Candidato/CandidatoLogin.razor` | Minimal (uses custom CSS classes) |
| `Components/Pages/Error.razor` | Already uses Bootstrap `text-danger` |
| `Components/Pages/NotFound.razor` | Minimal |
| `Components/Layout/MainLayout.razor.css` | Add Bootstrap-compatible overrides |
| `Components/Layout/NavMenu.razor.css` | Add Bootstrap-compatible overrides |
| `wwwroot/app.css` | Update comment |
| `wwwroot/css/formulario.css` | Minor adjustments if needed |

## Custom CSS Needed

Some Tailwind utilities have no Bootstrap equivalent and will need custom CSS classes in `formulario.css` or `app.css`:

- `.bg-gray-50` → `background-color: #f9fafb;`
- `.bg-gray-100` → `background-color: #f3f4f6;`
- `.bg-gray-200` → `background-color: #e5e7eb;`
- `.bg-gray-500` → `background-color: #6b7280;`
- `.bg-gray-800` → `background-color: #1f2937;`
- `.bg-gray-900` → `background-color: #111827;`
- `.bg-indigo-500/600` → `background-color: #4f46e5;`
- `.bg-indigo-600` → `background-color: #4338ca;`
- `.bg-cyan-500/600` → `background-color: #0891b2;`
- `.bg-yellow-500` → `background-color: #eab308;`
- `.bg-green-500/600` → `background-color: #16a34a;`
- `.bg-red-500/600` → `background-color: #dc2626;`
- `.text-gray-400/500/600/700/800/900` → corresponding `color` values
- `.text-indigo-600` → `color: #4338ca;`
- `.text-indigo-500` → `color: #6366f1;`
- `.text-cyan-600` → `color: #0891b2;`
- `.text-yellow-500/600` → `color: #eab308;`
- `.text-green-600` → `color: #16a34a;`
- `.text-red-500/600` → `color: #dc2626;`
- `.text-blue-600` → `color: #2563eb;`
- `.hover\:bg-gray-50` → `.hover-bg-light:hover`
- `.hover\:bg-gray-700` → `.hover-bg-gray-700:hover`
- `.hover\:text-white` → `.hover-text-white:hover`
- `.hover\:text-indigo-800` → `.hover-text-indigo-800:hover`
- `.hover\:text-gray-700` → `.hover-text-gray-700:hover`
- `.hover\:text-gray-500` → `.hover-text-gray-500:hover`
- `.hover\:text-red-800` → `.hover-text-red-800:hover`
- `.hover\:text-blue-700` → `.hover-text-blue-700:hover`
- `.hover\:bg-blue-700` → `.hover-bg-blue-700:hover`
- `.hover\:bg-red-700` → `.hover-bg-red-700:hover`
- `.hover\:bg-green-700` → `.hover-bg-green-700:hover`
- `.hover\:bg-gray-600` → `.hover-bg-gray-600:hover`
- `.hover\:bg-cyan-700` → `.hover-bg-cyan-700:hover`
- `.hover\:shadow-md` → `.hover-shadow-md:hover`
- `.hover\:border-blue-500` → `.hover-border-blue-500:hover`
- `.hover\:border-gray-200` → `.hover-border-gray-200:hover`
- `.transition-colors` → `transition: color 0.15s ease, background-color 0.15s ease;`
- `.animate-spin` → `@keyframes spin { to { transform: rotate(360deg); } } .animate-spin { animation: spin 1s linear infinite; }`
- `.border-l-3` → `border-left-width: 3px;`
- `.border-l-transparent` → `border-left-color: transparent;`
- `.border-transparent` → `border-color: transparent;`
- `.min-h-screen` → `min-height: 100vh;`
- `.min-h-\[80vh\]` → `min-height: 80vh;`
- `.min-h-\[600px\]` → `min-height: 600px;`
- `.h-\[600px\]` → `height: 600px;`
- `.h-5/6` → `height: 83.333333%;`
- `.h-full` → `height: 100%;`
- `.w-11/12` → `width: 91.666667%;`
- `.w-\[90%\]` → `width: 90%;`
- `.max-w-\[32rem\]` → `max-width: 32rem;`
- `.max-w-\[20rem\]` → `max-width: 20rem;`
- `.max-h-\[80vh\]` → `max-height: 80vh;`
- `.max-h-\[20rem\]` → `max-height: 20rem;`
- `.max-w-5xl` → `max-width: 64rem;`
- `.max-w-6xl` → `max-width: 72rem;`
- `.max-w-7xl` → `max-width: 80rem;`
- `.max-w-2xl` → `max-width: 42rem;`
- `.max-w-md` → `max-width: 28rem;`
- `.max-w-\[90%\]` → `max-width: 90%;`
- `.sticky-top` → `position: sticky; top: 0;`
- `.sticky-top-\[6px\]` → `position: sticky; top: 6px;`
- `.fixed-inset-0` → `position: fixed; inset: 0;`
- `.bg-black\/50` → `background-color: rgba(0,0,0,0.5);`
- `.bg-black\.bg-opacity-50` → `background-color: rgba(0,0,0,0.5);`
- `.bg-blue-50` → `background-color: #eff6ff;`
- `.bg-green-50` → `background-color: #f0fdf4;`
- `.bg-red-50` → `background-color: #fef2f2;`
- `.bg-yellow-50` → `background-color: #fefce8;`
- `.bg-cyan-50` → `background-color: #ecfeff;`
- `.bg-indigo-50` → `background-color: #eef2ff;`
- `.bg-gray-100` → `background-color: #f3f4f6;`
- `.bg-white\.rounded-lg` → already covered by `.bg-white.rounded-lg`
- `.text-4xl` → `font-size: 2.25rem;` (Bootstrap `fs-1` is 2.5rem)
- `.text-5xl` → `font-size: 3rem;`
- `.text-xs` → `font-size: 0.75rem;` (Bootstrap `fs-7`)
- `.text-sm` → `font-size: 0.875rem;` (Bootstrap `fs-6`)
- `.text-base` → `font-size: 1rem;` (Bootstrap `fs-base`)
- `.text-lg` → `font-size: 1.125rem;` (Bootstrap `fs-4`)
- `.text-xl` → `font-size: 1.25rem;` (Bootstrap `fs-3`)
- `.text-2xl` → `font-size: 1.5rem;` (Bootstrap `fs-2`)
- `.text-3xl` → `font-size: 1.875rem;` (Bootstrap `fs-1`)
- `.space-y-4` → `> * + * { margin-top: 1rem; }`
- `.space-y-3` → `> * + * { margin-top: 0.75rem; }`
- `.gap-5` → `gap: 1.25rem;` (Bootstrap 5.3 supports `gap-5`)
- `.gap-3` → `gap: 0.75rem;` (Bootstrap supports `gap-3`)
- `.gap-2` → `gap: 0.5rem;` (Bootstrap supports `gap-2`)
- `.gap-1` → `gap: 0.25rem;` (Bootstrap supports `gap-1`)
- `.rounded-3xl` → `border-radius: 1rem;` (Bootstrap `rounded-pill` is too round)
- `.rounded-xl` → `border-radius: 0.75rem;` (Bootstrap doesn't have `rounded-xl`)
- `.rounded-sm` → `border-radius: 0.125rem;` (Bootstrap `rounded` is 0.25rem)
- `.border-l-3` → `border-left: 3px solid;`
- `.list-none` → `list-style: none;` (Bootstrap `list-unstyled`)
- `.no-underline` → `text-decoration: none;`
- `.cursor-pointer` → `cursor: pointer;`
- `.cursor-not-allowed` → `cursor: not-allowed;`
- `.appearance-none` → `appearance: none;`
- `.accent-color` → `accent-color: #2563eb;`
- `.opacity-90` → `opacity: 0.9;`
- `.opacity-75` → `opacity: 0.75;`
- `.z-50` → `z-index: 50;`
- `.shadow` → Bootstrap's `shadow` class
- `.shadow-sm` → Bootstrap's `shadow-sm` class
- `.shadow-md` → Custom `.shadow-md { box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1), 0 2px 4px -2px rgba(0,0,0,0.1); }`
- `.shadow-lg` → Bootstrap's `shadow-lg` class

## Verification

1. Build the project: `dotnet build src/frontend/ProcessoSelecao.Blazor/ProcessoSelecao.Blazor.csproj`
2. Run the project and visually verify all pages render correctly with Bootstrap styling
3. Check that no Tailwind CSS CDN script is loaded in the browser
4. Verify Bootstrap CSS and JS are loaded in the browser
5. Test responsive behavior across breakpoints
6. Verify all interactive elements (buttons, forms, modals, navigation) work correctly

## Risks

- **Class mapping gaps**: Some Tailwind utilities (e.g., `bg-gradient-to-br`, `from-indigo-500`, `to-purple-600`, `border-l-3`) have no Bootstrap equivalent and require custom CSS
- **Responsive prefixes**: Tailwind's `lg:`, `md:`, `sm:` prefixes map to Bootstrap's responsive grid classes differently — the `grid`/`col-span` approach needs to be converted to Bootstrap's `row`/`col` system
- **`transition-colors`**: Bootstrap does not have a utility for CSS transitions on color changes — custom CSS needed
- **`animate-spin`**: Bootstrap does not include a spin animation — custom `@keyframes` needed
- **`space-y-*`**: Bootstrap 5.3 added `gap` utilities but not vertical spacing utilities like Tailwind's `space-y` — custom CSS needed
- **`hover:` variants**: Many `hover:` Tailwind classes need custom CSS equivalents in Bootstrap
- **`focus:` variants**: Tailwind's `focus:ring-2 focus:ring-blue-200` pattern has no Bootstrap equivalent — Bootstrap's `form-control:focus` handles this differently
- **Form controls**: Tailwind's `focus:outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-200` pattern needs to be replaced with Bootstrap's `form-control` which has its own focus styling

## Notes

- The `Error.razor` page already uses Bootstrap's `text-danger` class, confirming Bootstrap compatibility
- The `Counter.razor` page already uses `btn btn-primary`, confirming Bootstrap is the intended target
- Bootstrap's `form-control` class replaces the need for custom input styling in most cases
- Bootstrap's `btn` classes replace the need for custom button styling in most cases
- Bootstrap's `alert` classes replace the need for custom alert/notification styling